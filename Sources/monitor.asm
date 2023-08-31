;-------------------------------------------------------------------------------;
; RC2014 Monitor                                                                ;  
; by Dirk Prins                                                                 ;
; ForNext Software Development                                                  ;
; info@fornext.nl                                                               ;
;-------------------------------------------------------------------------------;

;-------------------------------------------------------------------------------;
; Defines                                                                       ;
;-------------------------------------------------------------------------------;

; ASCII
LF                              .EQU    $0A
FF                              .EQU    $0C
CR                              .EQU    $0D
ESC                             .EQU    $1B

; Serial Interface Buffer Size
SIO_BUFFER_SIZE                 .EQU    $40

; Serial Interface Ports
SIO_A_COMMAND                   .EQU    $80
SIO_A_DATA                      .EQU    $81
SIO_B_COMMAND                   .EQU    $82
SIO_B_DATA                      .EQU    $83

; Stack location (for monitor program only, will be altered by BIOS)
STACK                           .EQU    $A000         

;-------------------------------------------------------------------------------;
; Start of the monitor program                                                  ;
;-------------------------------------------------------------------------------;
.ORG    $0000

;-------------------------------------------------------------------------------;
; Reset                                                                         ;
;-------------------------------------------------------------------------------;
RST00:  DI                              ; Disable interrupts
        JP   INIT                       ; Initialize Hardware 

;-------------------------------------------------------------------------------;
; Serial Input Output interrupt vector ($40)                                    ;
;-------------------------------------------------------------------------------;
        .ORG $0040                      ; Holds the address of the 
        .DW  IO_INTERRUPT_HANDLER       ; interrupt handler for the Serial I/O 

;-------------------------------------------------------------------------------;
; Initialize hardware                                                           ;
;-------------------------------------------------------------------------------;
INIT:   LD   SP,STACK                   ; Set the Stack Pointer
        LD   HL,SIO_BUFFER              ; Set the SIO buffer pointers
        LD   (SIO_BUFFER_PTR),HL

        ; Initialise SIO A
        LD   A,$00                      ; Select Register 0 (WR0) 
        OUT  (SIO_A_COMMAND),A
        LD   A,$18                      ; Channel Reset
        OUT  (SIO_A_COMMAND),A

        LD   A,$04                      ; Select Register 4 (WR4) 
        OUT  (SIO_A_COMMAND),A
        LD   A,$C4                      ; X64 Clock Mode, 8-Bit Character, 1 Stop Bit, No Parity 
        OUT  (SIO_A_COMMAND),A

        LD   A,$01                      ; Select Register 1 (WR1) 
        OUT  (SIO_A_COMMAND),A
        LD   A,$18                      ; Interrupt On All Receive Characters, parity error is not a special receive condition 
        OUT  (SIO_A_COMMAND),A

        LD   A,$03                      ; Select Register 3 (WR3)
        OUT  (SIO_A_COMMAND),A
        LD   A,$E1                      ; Rx 8 Bits/Character, Auto Enables, RX on
        OUT  (SIO_A_COMMAND),A          

        LD   A,$05                      ; Select Register 5 (WR5)
        OUT  (SIO_A_COMMAND),A
        LD   A,$EA                      ; DTR active, 8-Bit Character, SEND BREAK off, TX on, RTS active
        OUT  (SIO_A_COMMAND),A

        ; Initialise SIO B
        LD   A,$00                      ; Select Register 0 (WR0) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$18                      ; Channel Reset
        OUT  (SIO_B_COMMAND),A

        LD   A,$04                      ; Select Register 4 (WR4) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$C4                      ; X64 Clock Mode, 8-Bit Character, 1 Stop Bit, No Parity 
        OUT  (SIO_B_COMMAND),A

        LD   A,$01                      ; Select Register 1 (WR1) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$18                      ; Interrupt On All Receive Characters, parity error is not a special receive condition 
        OUT  (SIO_B_COMMAND),A

        LD   A,$02                      ; Select Register 2 (WR2) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$40                      ; Write Interrupt Vector for both channels
        OUT  (SIO_B_COMMAND),A
        
        LD   A,$03                      ; Select Register 3 (WR3)
        OUT  (SIO_B_COMMAND),A
        LD   A,$E1                      ; Rx 8 Bits/Character, Auto Enables, RX on
        OUT  (SIO_B_COMMAND),A

        LD   A,$05                      ; Select Register 5 (WR5)
        OUT  (SIO_B_COMMAND),A
        LD   A,$EA                      ; DTR active, 8-Bit Character, SEND BREAK off, TX on, RTS active
        OUT  (SIO_B_COMMAND),A

        LD   HL,BOOTING_TEXT            ; Output notification to console
        CALL PRINT
        
        CALL COPY_BIOS                  ; Copy bios from ROM to RAM

        LD   HL,COPY_DONE_TEXT          ; Output notification to console
        CALL PRINT

        LD   A,$00                      ; Interrupt vector in page 0
        LD   I,A
        IM   2                          ; Interrupt mode 2
        EI

        JP   MAIN                       ; Jump to command interpreter routine

;-------------------------------------------------------------------------------;
; Serial input/output routines                                                  ;
; These routines will be called to (de)activate the RTS signal                  ;
;-------------------------------------------------------------------------------;
A_RTS_OFF:
        DI
        LD   A,$05                      ; Write into WR0: select WR5
        OUT  (SIO_A_COMMAND),A
        LD   A,$E8                      ; DTR active, 8-Bit Character, SEND BREAK off, TX on, RTS inactive
        OUT  (SIO_A_COMMAND),A
        EI
        RET

A_RTS_ON:
        DI
        LD   A,$05                      ; Write into WR0: select WR5
        OUT  (SIO_A_COMMAND),A
        LD   A,$EA                      ; DTR active, 8-Bit Character, SEND BREAK off, TX on, RTS active
        OUT  (SIO_A_COMMAND),A
        EI
        RET

;-------------------------------------------------------------------------------;
; Serial input/output routines                                                  ;
; This routine will be called to check for RX buffer empty                      ;
;-------------------------------------------------------------------------------;
A_RX_EMP:
        LD   A,$00                      ; Clear A, write into WR0: select RR0
        OUT  (SIO_A_COMMAND),A
        IN   A,(SIO_A_COMMAND)          ; Read status
        BIT  0,A
        RET  Z                          ; If any rx char left in rx buffer:
        IN   A,(SIO_A_DATA)             ; read that char
        JR   A_RX_EMP

;-------------------------------------------------------------------------------;
; Serial Input Output interrupt handler                                         ;
; This routine will be called if a character is received from SIO_A or SIO_B    ;
;-------------------------------------------------------------------------------;
IO_INTERRUPT_HANDLER:
        PUSH AF
        PUSH HL
        PUSH BC
        CALL A_RTS_OFF                                  ; Disable receive
        LD   HL,(SIO_BUFFER_PTR)                        ; Get location for new char in the buffer 
        LD   A,L
        CP   (SIO_BUFFER + SIO_BUFFER_SIZE) & $FF       ; Check if there is a buffer overrun
        JR   NZ,GET_CHAR_SIO_A                          ; No overrun, so get char        
        LD   HL,BUFFER_OVERRUN_TEXT                     ; Indicate buffer overrun
        CALL PRINT
        LD   HL,SIO_BUFFER                              ; Clear buffer (set to start)
        LD   (SIO_BUFFER_PTR),HL
        JR   IO_INTERRUPT_EXIT                          ; Exit 

GET_CHAR_SIO_A:
        IN   A,(SIO_A_DATA)                             ; Get char from A
        LD   (HL),A                                     ; Put in buffer
        INC  HL                                         ; Point to next location in buffer
        LD   (SIO_BUFFER_PTR),HL                        ; Set pointer to next location in buffer
        CALL CONSOLE_OUT                                ; Echo to console

IO_INTERRUPT_EXIT:        
        CALL A_RX_EMP                                   ; Flush receive buffer
        CALL A_RTS_ON                                   ; Enable receive
        POP  BC
        POP  HL
        POP  AF
        EI
        RETI

;-------------------------------------------------------------------------------;
; Main loop                                                                     ;
; Check for commands from the console and execute them                          ;
;-------------------------------------------------------------------------------;
MAIN:   CALL CONSOLE_IN                 ; Get last character from the input buffer
        CP   CR                         ; Compare to CR
        JR   NZ,MAIN
        LD   A,LF
        OUT  (SIO_A_DATA),A             ; Output LF
        LD   HL,SIO_BUFFER              ; Get pointer to first char entered
        LD   A,(HL)                     ; Get char
COMMAND_GO:
        CP   'g'                        ; Compare to 'g'
        JR   NZ,COMMAND_RESET           ; Not recognized
        JP   BIOS_RAM                   ; Goto BIOS
COMMAND_RESET:
        CP   'r'                        ; Compare to 'r'
        JR   NZ,UNKNOWN_COMMAND         ; Not recognized
        RST  $00                        ; Goto Reset
UNKNOWN_COMMAND:
        LD   HL,UNKNOWN_COMMAND_TEXT 
        CALL PRINT
        LD   HL,SIO_BUFFER              ; Clear buffer (set to start)
        LD   (SIO_BUFFER_PTR),HL
        JR   MAIN

;-------------------------------------------------------------------------------;
; Get the last entered character from the input buffer                          ;
;-------------------------------------------------------------------------------;
CONSOLE_IN:
        LD   HL,(SIO_BUFFER_PTR)        ; Get pointer to first empty space in buffer
        LD   A,LOW(SIO_BUFFER)          ; Low byte of buffer start address to A
        CP   L                          ; Check if any chars entered 
        JR   Z,CONSOLE_IN               ; No chars in buffer, so wait for it
        DEC  HL                         ; Point to last char entered
        LD   A,(HL)                     ; Get char
        RET

;-------------------------------------------------------------------------------;
; Output a character in register A to the console                               ;
;-------------------------------------------------------------------------------;
CONSOLE_OUT:
        PUSH AF                         ; Store character
CONSOLE_OUT1:    
        LD   A,$00
        OUT  (SIO_A_COMMAND),A
        IN   A,(SIO_A_COMMAND)          ; Get status byte, bit 2 = TX ready    
        BIT  2,A                        ; See if SIO is finished transmitting  
        JR   Z,CONSOLE_OUT1             ; Loop until SIO flag signals Ready
        POP  AF                         ; Retrieve character
        OUT  (SIO_A_DATA),A             ; Output the character
        RET

;-------------------------------------------------------------------------------;
; Print string of characters pointed to by HL to the console until byte = $00   ;
;-------------------------------------------------------------------------------;
PRINT:  
        LD   A,(HL)                     ; Get character
        CP   $00                        ; Is it $00 ?
        RET  Z                          ; Then Return ($00 is terminator)
        CALL CONSOLE_OUT                ; Print it
        INC  HL                         ; Next Character
        JR   PRINT                      ; Continue until $00

;-------------------------------------------------------------------------------;
; Copy bios code from ROM to RAM                                                ;
;-------------------------------------------------------------------------------;
COPY_BIOS:
        LD   HL,BIOS_ROM
        LD   DE,BIOS_RAM
        LD   BC,BIOS_SIZE
        LDIR
        RET

;-------------------------------------------------------------------------------;
; Messages to print to console                                                  ;
;-------------------------------------------------------------------------------;

BOOTING_TEXT:
        .DB  $0D,$0A
        .DB  "BOOTING..."
        .DB  $0D,$0A,$00

COPY_DONE_TEXT:
        .DB  $0D,$0A
        .DB  "The copying of the BIOS code has finished :-)"
        .DB  $0D,$0A
        .DB  "go or reset ?"
        .DB  $0D,$0A,'>',$00

BUFFER_OVERRUN_TEXT:
        .DB  $0D,$0A
        .DB  "WARNING: Keyboard Buffer Overrun"
        .DB  $0D,$0A,'>',$00

UNKNOWN_COMMAND_TEXT:
        .DB  $0D,$0A
        .DB  "ERROR: Unknown Command"
        .DB  $0D,$0A,'>',$00

STATUS_TEXT:
        .DB  $0D,$0A
        .DB  "Status RR0/RR1: "
        .DB  $00

END_OF_MONITOR:
        HALT                            ; Needed to include the last $00 in a binary save

;-------------------------------------------------------------------------------;
; Start of Bios code in ROM and RAM                                             ;
; This bios code must be compiled from a seperate source (bios.asm)             ;
; and binary inserted at the end of this monitor in the ROM image               ;
; The monitor will copy this code from ROM to RAM (max size bios = 0x1A00)      ;
; Then the monitor will start the bios from the RAM                             ;
; First thing it will do is disable the ROM so the full 64 KB of RAM            ;
; will be available to the system, then it will load CP/M from Compact Flash    ;
;-------------------------------------------------------------------------------;
BIOS_ROM        .EQU  $
BIOS_RAM        .EQU  $E600                     
BIOS_SIZE       .EQU  $1A00

;-------------------------------------------------------------------------------;
; Buffers in RAM area  (first 32K is occupied by ROM)                           ;
; We only need the labels (addresses)                                           ;
;-------------------------------------------------------------------------------;
.ORG    $8000

; Serial Interface parameters 
SIO_BUFFER:                     .DS     SIO_BUFFER_SIZE
SIO_BUFFER_PTR:                 .DS     2

.END    
