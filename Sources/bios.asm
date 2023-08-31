;-------------------------------------------------------------------------------;
; RC2014 Bios                                                                   ;  
; by Dirk Prins                                                                 ;
; ForNext Software Development                                                  ;
; info@fornext.nl                                                               ;
;-------------------------------------------------------------------------------;

;-------------------------------------------------------------------------------;
; Defines                                                                       ;
;-------------------------------------------------------------------------------;

; CP/M addresses for variables
IOBYTE                          .EQU    $0003           ; I/O definition byte
TDRIVE                          .EQU    $0004           ; Current user and drive number
ENTRY                           .EQU    $0005           ; Entry point for the cp/m bdos
TFCB                            .EQU    $005C           ; Default file control block
TBUFF                           .EQU    $0080           ; I/O buffer and command line storage
TBASE                           .EQU    $0100           ; Transient program storage area

; ASCII
BS                              .EQU    $08             ; Backspace
TAB                             .EQU    $09             ; Tab
LF                              .EQU    $0A             ; Line Feed
FF                              .EQU    $0C             ; Form Feed
CR                              .EQU    $0D             ; Carriage Return
ESC                             .EQU    $1B             ; Escape

; CF Registers
CF_DATA                         .EQU    $10
CF_FEATURES                     .EQU    $11
CF_ERROR                        .EQU    $11
CF_SEC_COUNT                    .EQU    $12
CF_LBA0                         .EQU    $13
CF_LBA1                         .EQU    $14
CF_LBA2                         .EQU    $15
CF_LBA3                         .EQU    $16
CF_STATUS                       .EQU    $17
CF_COMMAND                      .EQU    $17

; CF Features
CF_FEATURE_ENABLE_8BIT          .EQU    $01
CF_FEATURE_DISABLE_CACHE        .EQU    $82

; CF Commands
CF_READ_SEC                     .EQU    $20
CF_WRITE_SEC                    .EQU    $30
CF_SET_FEATURES                 .EQU    $EF

; CP/M addresses
; The BDOS_START_ADDR depends on the cpm file version !
; It's the address of the 'FBASE' routine
CPM_LOAD_ADDR                   .EQU    $D000     
BDOS_START_ADDR                 .EQU    $D806

; Serial Interface Buffer Size
SIO_BUFFER_SIZE                 .EQU    $40

; Serial Interface Ports
SIO_A_COMMAND                   .EQU    $80
SIO_A_DATA                      .EQU    $81
SIO_B_COMMAND                   .EQU    $82
SIO_B_DATA                      .EQU    $83

;-------------------------------------------------------------------------------;
; Start of the BIOS program                                                     ;
;-------------------------------------------------------------------------------;

BIOS:                           .ORG    $E600

;-------------------------------------------------------------------------------;
; BIOS jump table                                                               ;
;-------------------------------------------------------------------------------;
        JP    BOOT                      ;  0 Initialize
WBOOT0: JP    WBOOT                     ;  1 Warm boot
        JP    CONST                     ;  2 Console status
        JP    CONIN                     ;  3 Console input
        JP    CONOUT                    ;  4 Console output
        JP    LIST                      ;  5 List output
        JP    PUNCH                     ;  6 Punch output
        JP    READER                    ;  7 Reader input
        JP    HOME                      ;  8 Home disk
        JP    SELDSK                    ;  9 Select disk
        JP    SETTRK                    ; 10 Select track
        JP    SETSEC                    ; 11 Select sector
        JP    SETDMA                    ; 12 Set Disk Memory Access address
        JP    READ                      ; 13 Read 128 bytes
        JP    WRITE                     ; 14 Write 128 bytes
        JP    LISTST                    ; 15 List status of printer
        JP    SECTRN                    ; 16 Sector translate

;-------------------------------------------------------------------------------;
; Cold boot                                                                     ;
;-------------------------------------------------------------------------------;
BOOT:   DI                              ; Disable interrupts
        LD   SP,STACK                   ; Set default stack
        LD   A,$01
        OUT  ($38),A                    ; Turn off ROM

        LD   A,$C3                      ; Opcode for 'JP' in A
        LD   ($00),A                    ; Set opcode in entry point for reset
        LD   HL,WBOOT0                  ; Address of jump for warm boot
        LD   ($01),HL

        LD   A,$01                      ; LIST is TTY:, PUNCH is TTY:, READER is TTY:, Console is CRT:
        LD   (IOBYTE),A
        
        LD   A,$00                      ; Set disk selector
        LD   (TDRIVE),A

        LD   HL,IO_INTERRUPT_HANDLER    ; Set the address for the interrupt handler      
        LD   (IO_INT_HAND_PTR),HL       ; for the Serial I/O (not really necessary) 

        LD   HL,TBUFF                   ; Set the address for the Disk Memory Access
        LD   (DMA_ADDR),HL              ; to the disk I/O buffer      

        LD   HL,SIO_BUFFER              ; Set the SIO buffer pointer
        LD   (SIO_BUFFER_PTR),HL

        LD   A,$C3                      ; Opcode for 'JP' in A
        LD   (ENTRY),A                  ; Set opcode in entry point for the CP/M BDOS
        LD   HL,BDOS_START_ADDR         ; Address of jump for the BDOS
        LD   (ENTRY + 1),HL

        ; Initialise SIO Channel A
        LD   A,$00                      ; Select Register 0 (WR0) 
        OUT  (SIO_A_COMMAND),A
        LD   A,$18                      ; Channel Reset
        OUT  (SIO_A_COMMAND),A

        LD   A,$04                      ; Select Register 4 (WR4) 
        OUT  (SIO_A_COMMAND),A
        LD   A,$C4                      ; X64 Clock Mode, 8-Bit SYNC Character, 1 Stop Bit, No Parity 
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
        LD   A,$EA                      ; DTR active, 8-Bit SYNC Character, SEND BREAK off, TX on, RTS active
        OUT  (SIO_A_COMMAND),A

        ; Initialise SIO Channel B
        LD   A,$00                      ; Select Register 0 (WR0) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$18                      ; Channel Reset
        OUT  (SIO_B_COMMAND),A

        LD   A,$04                      ; Select Register 4 (WR4) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$C4                      ; X64 Clock Mode, 8-Bit SYNC Character, 1 Stop Bit, No Parity 
        OUT  (SIO_B_COMMAND),A

        LD   A,$01                      ; Select Register 1 (WR1) 
        OUT  (SIO_B_COMMAND),A
        LD   A,$18                      ; Interrupt On All Receive Characters, parity error is not a special receive condition 
        OUT  (SIO_B_COMMAND),A

        LD   A,$02                      ; Select Register 0 (WR0) 
        OUT  (SIO_B_COMMAND),A
        LD   A,IO_INT_HAND_PTR          ; Write Interrupt Vector Channel A and B (low byte, high byte is the by way of the interrupt register)
        OUT  (SIO_B_COMMAND),A
        
        LD   A,$03                      ; Select Register 3 (WR3)
        OUT  (SIO_B_COMMAND),A
        LD   A,$E1                      ; Rx 8 Bits/Character, Auto Enables, RX on
        OUT  (SIO_B_COMMAND),A

        LD   A,$05                      ; Select Register 5 (WR5)
        OUT  (SIO_B_COMMAND),A
        LD   A,$EA                      ; DTR active, 8-Bit SYNC Character, SEND BREAK off, TX on, RTS active
        OUT  (SIO_B_COMMAND),A

        LD   A,$FF                      ; Interrupt vector in page $FF
        LD   I,A
        IM   2                          ; Interrupt mode 2
        EI

        LD   HL,BIOS_TEXT               ; Output notification to console
        CALL PRINT_CONSOLE

        LD   A,(TDRIVE)                 ; Get drive number
        LD   C,A                        ; Drive number to C

        JP   CPM_LOAD                   ; Load CP/M from compact flash 

;-------------------------------------------------------------------------------;
; Warm boot                                                                     ;
;-------------------------------------------------------------------------------;

WBOOT:  DI                              ; Disable interrupts
        LD   SP,STACK                   ; Set default stack

        LD   A,$01                      ; LIST is TTY:, PUNCH is TTY:, READER is TTY:, Console is CRT:
        LD   (IOBYTE),A
        
        LD   HL,IO_INTERRUPT_HANDLER    ; Set the address for the        
        LD   (IO_INT_HAND_PTR),HL       ; interrupt handler for the Serial I/O 

        LD   HL,TBUFF                   ; Set the address for the Disk Memory Access
        LD   (DMA_ADDR),HL              ; to the disk I/O buffer

        LD   HL,SIO_BUFFER              ; Set the SIO buffer pointer
        LD   (SIO_BUFFER_PTR),HL

        LD   A,$C3                      ; Opcode for 'JP' in A
        LD   (ENTRY),A                  ; Set opcode in entry point for the CP/M BDOS
        LD   HL,BDOS_START_ADDR         ; Address of jump for the BDOS
        LD   (ENTRY + 1),HL

        LD   A,$FF                      ; Interrupt vector in page $FF
        LD   I,A
        IM   2                          ; Interrupt mode 2
        EI

        LD   A,$00                      ; Set user/disk selector
        LD   (TDRIVE),A
        LD   C,A                        ; User/Drive number to C

        JP   CPM_LOAD                   ; Load CP/M from compact flash 

;-------------------------------------------------------------------------------;
; Print string of characters to the console until byte = $00                    ;
;-------------------------------------------------------------------------------;
PRINT_CONSOLE:  
        PUSH AF
        PUSH BC

PRINT_CONSOLE_REPEAT:
        LD   A,(HL)                     ; Get character
        CP   $00                        ; Is it $00 ?
        JR   Z,PRINT_CONSOLE_RETURN     ; Then Return ($00 is terminator)
        LD   C,A                        ; Put char in C
        CALL CONOUT                     ; Print it
        INC  HL                         ; Next Character
        JR   PRINT_CONSOLE_REPEAT       ; Continue until $00

PRINT_CONSOLE_RETURN:
        POP  BC
        POP  AF
        RET

;-------------------------------------------------------------------------------;
; Load CPM from compact flash                                                   ;
;-------------------------------------------------------------------------------;
CPM_LOAD:
        LD   HL,CPM_LOADING_TEXT        ; Inform user we are loading CPM
        CALL PRINT_CONSOLE

        CALL CF_WAIT
        LD   A,CF_FEATURE_ENABLE_8BIT   ; Enable 8-bit data transfers
        OUT  (CF_FEATURES),A            
        LD   A,CF_SET_FEATURES
        OUT  (CF_COMMAND),A

        CALL CF_WAIT
        LD   A,CF_FEATURE_DISABLE_CACHE ; Disable Write Cache
        OUT  (CF_FEATURES),A            
        LD   A,CF_SET_FEATURES
        OUT  (CF_COMMAND),A

        LD   B,11                       ; Load 11 Sectors of 512 bytes (max CP/M size = 5.5 KB)
        LD   A,0
        LD   (SECTOR_NUMBER_LBA0),A     ; Set sector number to read
        LD   DE,CPM_LOAD_ADDR           ; Set destination address

PROCESS_SECTORS:
        CALL CF_WAIT
        LD   A,(SECTOR_NUMBER_LBA0)     ; Set sector number to read
        OUT  (CF_LBA0),A
        LD   A,0                        ; Only need LBA0, so LBA1, LBA2 are 0
        OUT  (CF_LBA1),A                
        OUT  (CF_LBA2),A                
        LD   A,$E0                      ; Set to Master, LBA Mode Access
        OUT  (CF_LBA3),A
        LD   A,1
        OUT  (CF_SEC_COUNT),A           ; Number Of Sectors (512 bytes each) to transfer
        CALL CF_READ_SECTOR             ; Read 1 sector in the CF_BUF
        PUSH BC
        LD   HL,CF_BUF                  ; Set source address
        LD   BC,512                     ; Copy size
        LDIR                            ; Copy block
        LD   A,(SECTOR_NUMBER_LBA0)     ; Increase sector number to read
        INC  A
        LD   (SECTOR_NUMBER_LBA0),A
        POP  BC
        DJNZ PROCESS_SECTORS            ; Repeat until all sectors read

        LD   HL,CPM_STARTING_TEXT       ; Inform user we are ready
        CALL PRINT_CONSOLE

        LD  A,(TDRIVE)                  ; Get drive number
        LD  C,A                         ; Drive number to C

        JP  CPM_LOAD_ADDR               ; Start CP/M 

;-------------------------------------------------------------------------------;
; Read sector from compact flash card (512 bytes)                               ;
;-------------------------------------------------------------------------------;
CF_READ_SECTOR:
        PUSH AF
        PUSH BC
        PUSH HL
        CALL CF_WAIT            ; Wait for disk to be ready
        LD   A,CF_READ_SEC
        OUT  (CF_COMMAND),A     ; Give command to read a sector
        CALL CF_WAIT            ; Wait for disk to be ready
        LD   BC,512             ; Number of bytes to read (512 = 1 sector)
        LD   HL,CF_BUF          ; Read CF data buffer address

CF_READ_BYTE:
        NOP
        NOP
        IN   A,(CF_DATA)        ; Read one byte
        LD   (HL),A             ; Store in memory
        INC  HL                 ; Increase memory pointer
        DEC  BC                 ; Decrease byte counter
        LD   A,C                ; Test if C from counter (BC) is 0
        CP   $00
        JR   NZ,CF_READ_BYTE    ; If not, read another byte
        LD   A,B                ; Test if B from counter (BC) is 0
        CP   $00
        JR   NZ,CF_READ_BYTE    ; If not, read another byte
        
        POP  HL
        POP  BC
        POP  AF
        RET

;-------------------------------------------------------------------------------;
; Write sector to compact flash card (512 bytes)                               ;
;-------------------------------------------------------------------------------;
CF_WRITE_SECTOR:
        PUSH AF
        PUSH BC
        PUSH HL
        CALL CF_WAIT            ; Wait for disk to be ready
        LD   A,CF_WRITE_SEC
        OUT  (CF_COMMAND),A     ; Give command to write a sector
        CALL CF_WAIT            ; Wait for disk to be ready
        LD   BC,512             ; Number of bytes to write (512 = 1 sector)
        LD   HL,CF_BUF          ; Read CF data buffer address

CF_WRITE_BYTE:
        NOP
        NOP
        LD   A,(HL)             ; Load from memory
        OUT  (CF_DATA),A        ; Write one byte
        INC  HL                 ; Increase memory pointer
        DEC  BC                 ; Decrease byte counter
        LD   A,C                ; Test if C from counter (BC) is 0
        CP   $00
        JR   NZ,CF_WRITE_BYTE   ; If not, write another byte
        LD   A,B                ; Test if B from counter (BC) is 0
        CP   $00
        JR   NZ,CF_WRITE_BYTE   ; If not, write another byte
        
        POP  HL
        POP  BC
        POP  AF
        RET

; Wait for disk to be ready 
CF_WAIT:
        PUSH AF
CF_WAIT1:
        IN   A,(CF_STATUS)      ; Get status from disk
        BIT  7,A                ; Test busy flag
        JR   NZ,CF_WAIT1        ; Repeat if still busy
        IN   A,(CF_STATUS)      ; Get status from disk
        BIT  6,A                ; Test ready flag
        JR   Z,CF_WAIT1         ; Repeat if not ready
        POP  AF
        RET

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
        CALL A_RTS_OFF                                  ; Disable receive
        LD   HL,(SIO_BUFFER_PTR)                        ; Get location for new char in the buffer 
        LD   A,L
        CP   (SIO_BUFFER + SIO_BUFFER_SIZE) & $FF       ; Check if there is a buffer overrun
        JR   NZ,GET_CHAR_SIO_A                          ; No overrun, so get char        
        LD   HL,BUFFER_OVERRUN_TEXT                     ; Indicate buffer overrun
        CALL PRINT_CONSOLE
        LD   HL,SIO_BUFFER                              ; Clear buffer (set to start)
        LD   (SIO_BUFFER_PTR),HL
        JR   IO_INTERRUPT_EXIT                          ; Exit 

GET_CHAR_SIO_A:
        IN   A,(SIO_A_DATA)                             ; Get char from A
        LD   (HL),A                                     ; Put in buffer
        INC  HL                                         ; Point to next location in buffer
        LD   (SIO_BUFFER_PTR),HL                        ; Set pointer to next location in buffer

IO_INTERRUPT_EXIT:        
        CALL A_RX_EMP                                   ; Flush receive buffer
        CALL A_RTS_ON                                   ; Enable receive
        POP  HL
        POP  AF
        EI
        RETI

;-------------------------------------------------------------------------------;
; Check for a char from the console                                          ;
;-------------------------------------------------------------------------------;
CONIN:
        LD   HL,(SIO_BUFFER_PTR)        ; Get pointer to first empty space in buffer
        LD   A,LOW(SIO_BUFFER)          ; Low byte of buffer start address to A
        CP   L                          ; Check if any chars entered 
        JR   Z,CONIN                    ; No chars in buffer
        DEC  HL                         ; Point to last char entered
        LD   A,(HL)                     ; Get char
        LD   (SIO_BUFFER_PTR),HL        ; Update pointer 
        RET

;-------------------------------------------------------------------------------;
; Output a character to the console                                             ;
;-------------------------------------------------------------------------------;
CONOUT:
        LD   A,(IOBYTE)                 ; Check if console output will go to CRT
        CP   $01
        RET  NZ                         ; Just return if not for CRT
        LD   A,$00
        OUT  (SIO_A_COMMAND),A
        IN   A,(SIO_A_COMMAND)          ; Get status byte, bit 2 = TX ready    
        BIT  2,A                        ; See if SIO is finished transmitting  
        JR   Z,CONOUT                   ; Loop until SIO flag signals Ready
        LD   A,C                        ; Char to A
        OUT  (SIO_A_DATA),A             ; Output the character
        RET

;-------------------------------------------------------------------------------;
; Console status                                                                ;
;-------------------------------------------------------------------------------;
CONST:
        LD   A,(IOBYTE)
        AND  00000011b                  ; Mask for console
        CP   00000001b                  ; Check if CRT should be used
        JR   NZ,EMPTY                   ; Not used, so set empty
        PUSH HL
        LD   HL,(SIO_BUFFER_PTR)        ; Get pointer to first empty space in buffer
        LD   A,LOW(SIO_BUFFER)          ; Low byte of buffer start address to A
        CP   L                          ; Check if any chars entered 
        JR   Z,EMPTY                    ; No chars in buffer
        LD   A,$FF                      ; Set status
        POP  HL
        RET

EMPTY:  LD      A,$00
        POP     HL
        RET

;-------------------------------------------------------------------------------;
; List output                                                                   ;
;-------------------------------------------------------------------------------;
LIST:
        LD   A,(IOBYTE)                 ; Check if list output will go to CRT
        CP   $04
        RET  NZ                         ; Just return if not for CRT
        LD   A,$00
        OUT  (SIO_A_COMMAND),A
        IN   A,(SIO_A_COMMAND)          ; Get status byte D0 = RX char ready    
        AND  $01                      
        CP   $01                        ; See if SIO is finished transmitting  
        JR   NZ,CONOUT                  ; Loop until SIO flag signals Ready
        LD   A,C                        ; Char to A
        OUT  (SIO_A_DATA),A             ; Output the character
        CP   CR                         ; If Cariage Return, output LineFeed as well
        RET  NZ
        LD   A,LF
        OUT  (SIO_A_DATA),A             ; Output the character
        RET

;-------------------------------------------------------------------------------;
; Punch output                                                                  ;
;-------------------------------------------------------------------------------;
PUNCH:
        RET                             ; Ignore all output for punch

;-------------------------------------------------------------------------------;
; Reader input                                                                  ;
;-------------------------------------------------------------------------------;
READER:
        RET                             ; Ignore all input for reader

;-------------------------------------------------------------------------------;
; Home disk                                                                     ;
;-------------------------------------------------------------------------------;
HOME:
        LD  BC,$0000                    ; Move disk head to track 0
        RET

;-------------------------------------------------------------------------------;
; Select disk                                                                   ;
;-------------------------------------------------------------------------------;
SELDSK:
        LD   HL,$0000                   ; Set disk parameter header to 0 (no disk)
        LD   A,(TDRIVE)                 ; Get current drive
        CP   C                          ; If this is not the slected drive
        RET  NZ                         ; then return (HL = no disk) 
        RLC  A                          ; *2 (multiply by 16 to get the entry 
        RLC  A                          ; *4  in the disk parameter block)  
        RLC  A                          ; *8
        RLC  A                          ; *16
        LD   HL,DISK_PARAMETER_TABLE    ; Load start address of disk parameter block
        LD   B,$00
        LD   C,A
        ADD  HL,BC                      ; Add offset
        RET

;-------------------------------------------------------------------------------;
; Select track                                                                  ;
;-------------------------------------------------------------------------------;
SETTRK: LD   (TRACK),BC                 ; Set track passed from BDOS 
        RET                             ; in register BC

;-------------------------------------------------------------------------------;
; Select sector                                                                 ;
;-------------------------------------------------------------------------------;
SETSEC: LD   (SECTOR),BC                ; Set sector passed from BDOS 
        RET                             ; in register BC

;-------------------------------------------------------------------------------;
; Set DMA address                                                               ;
;-------------------------------------------------------------------------------;
SETDMA: LD   (DMA_ADDR),BC              ; Set DMA address given by BC
        RET

;-------------------------------------------------------------------------------;
; Read 128 bytes                                                                ;
;-------------------------------------------------------------------------------;
READ:   
        CALL CONVERT_TO_LBA             ; Convert Disk, Track and Sector to LBA values
        LD   A,(SECTOR_NUMBER_LBA0)     ; Set sector number to read (singles)
        OUT  (CF_LBA0),A
        LD   A,(SECTOR_NUMBER_LBA1)     ; Set sector number to read (256's)
        OUT  (CF_LBA1),A                
        LD   A,(SECTOR_NUMBER_LBA2)     ; Set sector number to read (65536's)
        OUT  (CF_LBA2),A                
        LD   A,$E0                      ; Set to Master, LBA Mode Access
        OUT  (CF_LBA3),A
        LD   A,1
        OUT  (CF_SEC_COUNT),A           ; Number Of Sectors (512 bytes each) To Transfer
        LD   HL,CF_BUF                  ; Get disk buffer address
        CALL CF_READ_SECTOR             ; Read 512 bytes (1 sector)
        LD   A,(SECTION)                ; Check section to copy to 
        LD   L,A                        ; Put in HL to calculate an offset 
        LD   H,$00                      ; for the source address
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL                      ; Offset now in HL
        LD   DE,CF_BUF                  ; Compact Flash buffer addrress now in DE
        ADD  HL,DE                      ; Add address to offset 
        LD   DE,(DMA_ADDR)              ; Get disk memory address
        LD   B,128                      ; 128 bytes to copy

READ_128_BYTES:
        LD   A,(HL)                     ; Get source byte
        LD   (DE),A                     ; Put to destination
        INC  DE                         ; Increase adresses
        INC  HL
        DJNZ READ_128_BYTES             ; Decrease B and repeat until B=0

        IN   A,(CF_STATUS)              ; Get status from disk
        AND  $01                        ; Bit 0 is error bit
        RET  Z                          ; Return on no error

        IN   A,(CF_ERROR)               ; Get disk error number 
        INC  A                          ; Increase by 1 
        LD   B,A                        ; Move to B                                      
        LD   HL,IDE_ERROR_MESSAGES      ; Load start address of error messages

INCREASE_ERROR_READ:
        DEC  B                                                    
        JR   Z,PRINT_ERROR_READ         ; If 0, print error   
        LD   DE,$26                     ; Add 1 message length to start adrress
        ADD  HL,DE                     
        JR   INCREASE_ERROR_READ        ; Reapeat until 0 

PRINT_ERROR_READ:
        CALL PRINT_CONSOLE              ; Inform user of a read error 
        LD   A,$01                      ; Inform CP/M of a read error
        RET

;-------------------------------------------------------------------------------;
; Write 128 bytes                                                               ;
;-------------------------------------------------------------------------------;
WRITE:
        CALL CONVERT_TO_LBA             ; Convert Disk,Track,Sector to LBA values
        LD   A,(SECTOR_NUMBER_LBA0)     ; Set sector number to write (singles)
        OUT  (CF_LBA0),A
        LD   A,(SECTOR_NUMBER_LBA1)     ; Set sector number to write (256's)
        OUT  (CF_LBA1),A                
        LD   A,(SECTOR_NUMBER_LBA2)     ; Set sector number to write (65536's)
        OUT  (CF_LBA2),A                
        LD   A,$E0                      ; Set to Master, LBA Mode Access
        OUT  (CF_LBA3),A
        LD   A,1
        OUT  (CF_SEC_COUNT),A           ; Number Of Sectors (512 bytes each) To Transfer
        LD   HL,CF_BUF                  ; Get disk buffer address
        CALL CF_READ_SECTOR             ; First read 512 bytes (1 sector)
        LD   A,(SECTION)                ; Check section to copy to 
        LD   L,A                        ; Put in HL to calculate an offset 
        LD   H,$00                      ; for the destination address
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL
        ADD  HL,HL                      ; Offset now in HL
        LD   DE,CF_BUF                  ; Disk buffer address now in DE
        ADD  HL,DE                      ; Add address to offset 
        LD   DE,(DMA_ADDR)              ; Get disk memory address
        LD   B,128                      ; 128 bytes to copy

TRANSFER_128_BYTES:
        LD   A,(DE)                     ; Get source byte
        LD   (HL),A                     ; Put to destination
        INC  DE                         ; Increase adresses
        INC  HL
        DJNZ TRANSFER_128_BYTES         ; Decrease B and repeat until B=0

        CALL CF_WRITE_SECTOR            ; Write 512 bytes (1 sector)
        IN   A,(CF_STATUS)              ; Get status from disk
        AND  $01                        ; Bit 0 is error bit
        RET  Z                          ; Return on no error

        IN   A,(CF_ERROR)               ; Get disk error number 
        INC  A                          ; Increase by 1 
        LD   B,A                        ; Move to B                                      
        LD   HL,IDE_ERROR_MESSAGES      ; Load start address of error messages

INCREASE_ERROR_WRITE:
        DEC  B                                                    
        JR   Z,PRINT_ERROR_WRITE        ; If 0, print error   
        LD   DE,$26                     ; Add 1 message length to start adrress
        ADD  HL,DE                     
        JR   INCREASE_ERROR_WRITE       ; Reapeat until 0 

PRINT_ERROR_WRITE:
        CALL PRINT_CONSOLE              ; Inform user of a read error 
        LD   A,$01                      ; Inform CP/M of a read error
        RET

;-------------------------------------------------------------------------------;
; Convert Disk, Track and (CP/M) sector numbers to Compact Flash Sectors for    ;
; filling the LBA registers to read/write the Compact Flash card.               ;
;                                                                               ;
; For CP/M:                                                                     ;
; A disk is 16384 sectors                                                       ;
; A track is 128 sectors                                                        ;
; A sector is 128 bytes                                                         ;
;                                                                               ;
; For the Compact Flash card:                                                   ;
; A disk is 16384 sectors                                                       ;
; A track is 32 sectors                                                         ;
; A sector is 512 bytes                                                         ;
;-------------------------------------------------------------------------------;
CONVERT_TO_LBA:
        LD   A,$00
        LD   (SECTOR_NUMBER_LBA0),A     ; Clear sector number (1's => singles)
        LD   (SECTOR_NUMBER_LBA1),A     ; Clear sector number (256's)
        LD   (SECTOR_NUMBER_LBA2),A     ; Clear sector number (65536's)

CONVERT_DISK:
        LD   A,(TDRIVE)                 ; Get current drive number (16384 sectors each)
        AND  $0F                        ; Only low nibble valid (0 to 15 = A to P)
        CP   $00                        ; Check if done
        JR   Z,CONVERT_TRACK            ; Done, process tracks

CHECK_DISK:
        CP   $04                        ; Check if done
        JR   C,PROCESS_REMAINDER_DISK   ; Proces leftover ($00 to $03)
        LD   B,A                        ; Put in B 
        LD   A,(SECTOR_NUMBER_LBA2)     ; Get sector number (65536's)
        INC  A                          ; Add 65536 sectors 
        LD   (SECTOR_NUMBER_LBA2),A     ; Set sector number (65536's)
        LD   A,B                        ; Put leftover in A again
        DEC  A                          ; Decrease 4 disks (4 * 16384 sectors)
        DEC  A
        DEC  A
        DEC  A
        JR   NZ,CHECK_DISK              ; Not done yet, so repeat
        JR   CONVERT_TRACK              ; Done, process tracks

PROCESS_REMAINDER_DISK:
        SLA  A                          ; Low 3 bits are 256's, so for LBA1                           
        SLA  A
        SLA  A
        SLA  A                        
        SLA  A                        
        SLA  A                        
        LD   (SECTOR_NUMBER_LBA1),A     ; Set sector number (256's)

CONVERT_TRACK:
        LD   HL,(TRACK)                 ; Get current track (each 128 sectors of 128 bytes on CP/M = 32 sectors of 512 bytes on Compact Flash)
        SLA  H                          ; Low bit of H is for LBA1 (max 512 cp/m tracks of 128 sectors of 128 bytes each on disk, so only bit0 can be value 1)
        SLA  H                          ; This bit0 will be 256 tracks, so we have to shift 5 positions to get the 256's for LBA1
        SLA  H
        SLA  H
        SLA  H
        LD   A,(SECTOR_NUMBER_LBA1)     ; Get sector number (256's)
        ADD  H
        LD   (SECTOR_NUMBER_LBA1),A     ; Set sector number (256's)

        SRL  L                          ; High 5 bits of L are 256's, so for LBA1 
        SRL  L
        SRL  L
        LD   A,(SECTOR_NUMBER_LBA1)     ; Get sector number (256's)
        ADD  L
        LD   (SECTOR_NUMBER_LBA1),A     ; Set sector number (256's)

        LD   HL,(TRACK)                 ; Get current track again
        LD   A,L                        ; L to A
        AND  $07                        ; Low 3 bits of L are 32 sectors on Compact Flash, so for LBA0
        SLA  A                          
        SLA  A
        SLA  A
        SLA  A
        SLA  A
        LD   L,A                        ; A to L
        LD   A,(SECTOR_NUMBER_LBA0)     ; Get sector number (singles)
        ADD  L
        LD   (SECTOR_NUMBER_LBA0),A     ; Set sector number (singles)

CONVERT_SECTORS:  
        LD   A,(SECTOR)                 ; Get single sectors (max value = 128 because of 128 sectors/track, so only one byte to read)
        LD   B,A                        ; Put in B
        SRL  B                          ; Shift 2 times to get compact flash sector size (128 <-> 512)
        SRL  B
        AND  $03                        ; Check which 128 byte section is selected 
        LD   (SECTION),A                ; Save
        LD   A,(SECTOR_NUMBER_LBA0)     ; Get sector number  
        ADD  A,B                        ; Add single sectors
        LD   (SECTOR_NUMBER_LBA0),A     ; Set sector number 
        RET

;-------------------------------------------------------------------------------;
; List status of current printer device                                         ;
;-------------------------------------------------------------------------------;
LISTST:
        LD   A,$00                      ; Return status (not ready)
        RET

;-------------------------------------------------------------------------------;
; Sector translate                                                              ;
;-------------------------------------------------------------------------------;
SECTRN: 
        LD   H,B                        ; No skew,
        LD   L,C                        ; so return BC into HL
        RET                             ; (just one on one)

;-------------------------------------------------------------------------------;
; Serial Interface parameters                                                   ;
;-------------------------------------------------------------------------------;
SIO_BUFFER:             .DS     SIO_BUFFER_SIZE
SIO_BUFFER_PTR:         .DS     2

;-------------------------------------------------------------------------------;
; Compact Flash parameters                                                      ;
;-------------------------------------------------------------------------------;
SECTOR_NUMBER_LBA0:  .DS     1  ; 1 sector (512 bytes)
SECTOR_NUMBER_LBA1:  .DS     1  ; 256 sectors (128 KB)
SECTOR_NUMBER_LBA2:  .DS     1  ; 256 * 256 = 4096 sectors (2 MB)
DMA_ADDR:            .DS     2

;-------------------------------------------------------------------------------;
; Disk parameters                                                               ;
;-------------------------------------------------------------------------------;
TRACK:               .DS     2  ; Track number, CP/M, so 128 * 128 = 16KB
SECTOR:              .DS     2  ; Sector number, CP/M, so 128 bytes each
SECTION:             .DS     1  ; Section of CP/M sector (128 bytes) 
                                ; in Compact Flash sector (512 bytes)
                                ; so can be 0,1,2 or 3

;-------------------------------------------------------------------------------;
; Disk parameter table with 16 disk parameter headers                           ;
;-------------------------------------------------------------------------------;
DISK_PARAMETER_TABLE:
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_BOOT,$0000,ALV00
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV01
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV02
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV03
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV04
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV05
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV06
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV07
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV08
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV09
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV10
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV11
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV12
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV13
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_8MB, $0000,ALV14
        .DW $0000,$0000,$0000,$0000,DIR_BUF,DPB_2MB, $0000,ALV15

;-------------------------------------------------------------------------------;
; Disk data storage                                                    ;
;-------------------------------------------------------------------------------;
DIR_BUF:        .DS 128         ; A 128-byte scratch pad area for directory operations within BDOS
ALV00:          .DS 257         ; Allocation vector 0
ALV01:          .DS 257         ; Allocation vector 1
ALV02:          .DS 257         ; Allocation vector 2
ALV03:          .DS 257         ; Allocation vector 3
ALV04:          .DS 257         ; Allocation vector 4
ALV05:          .DS 257         ; Allocation vector 5
ALV06:          .DS 257         ; Allocation vector 6
ALV07:          .DS 257         ; Allocation vector 7
ALV08:          .DS 257         ; Allocation vector 8
ALV09:          .DS 257         ; Allocation vector 9
ALV10:          .DS 257         ; Allocation vector 10
ALV11:          .DS 257         ; Allocation vector 11
ALV12:          .DS 257         ; Allocation vector 12
ALV13:          .DS 257         ; Allocation vector 13
ALV14:          .DS 257         ; Allocation vector 14
ALV15:          .DS 257         ; Allocation vector 15

;-------------------------------------------------------------------------------;
; Disk Parameter Blocks                                                         ;
;-------------------------------------------------------------------------------;

; Boot disk (first drive) has a reserved track for CP/M
DPB_BOOT:                               
        .DW 128         ; SPT - Total number of sectors per track
        .DB 5           ; BSH - Data allocation block shift factor, determined by the data block allocation size
        .DB 31          ; BLM - Data allocation block mask (2[BSH-1]).
        .DB 1           ; EXM - Extent mask, determined by the data block allocation size and the number of disk blocks
        .DW 2043        ; DSM - Total storage capacity (in blocks) of the disk drive - 1 (2048 - 1 - 3(Boot))
        .DW 511         ; DRM - Total number of directory entries - 1
        .DB 240         ; AL0 - Reserved directory blocks for directory entries
        .DB 0           ; AL1 - Reserved directory blocks for directory entries
        .DW 0           ; CKS - DIR check vector size (DRM+1)/4 (0=fixed disk: no directory records are checked)
        .DW 1           ; OFF - Reserved tracks (offset)

; Standard disk (8MB)
DPB_8MB:
        .DW 128         ; SPT - Total number of sectors per track
        .DB 5           ; BSH - Data allocation block shift factor, determined by the data block allocation size
        .DB 31          ; BLM - Data allocation block mask (2[BSH-1]).
        .DB 1           ; EXM - Extent mask, determined by the data block allocation size and the number of disk blocks
        .DW 2047        ; DSM - Total storage capacity (in blocks) of the disk drive - 1 (2048 - 1)
        .DW 511         ; DRM - Total number of directory entries - 1
        .DB 240         ; AL0 - Reserved directory blocks for directory entries
        .DB 0           ; AL1 - Reserved directory blocks for directory entries
        .DW 0           ; CKS - DIR check vector size (DRM+1)/4 (0=fixed disk: no directory records are checked)
        .DW 0           ; OFF - Reserved tracks (offset)

; Last drive is smaller because CF is never a real 128MB (always a bit less)
DPB_2MB:
        .DW 128         ; SPT - Total number of sectors per track
        .DB 5           ; BSH - Data allocation block shift factor, determined by the data block allocation size
        .DB 31          ; BLM - Data allocation block mask (2[BSH-1]).
        .DB 1           ; EXM - Extent mask, determined by the data block allocation size and the number of disk blocks
        .DW 511         ; DSM - Total storage capacity (in blocks) of the disk drive - 1 (512 - 1)
        .DW 511         ; DRM - Total number of directory entries - 1
        .DB 240         ; AL0 - Reserved directory blocks for directory entries
        .DB 0           ; AL1 - Reserved directory blocks for directory entries
        .DW 0           ; CKS - DIR check vector size (DRM+1)/4 (0=fixed disk: no directory records are checked)
        .DW 0           ; OFF - Reserved tracks (offset)
        
;-------------------------------------------------------------------------------;
; Compact Flash Data Buffer                                                     ;
;-------------------------------------------------------------------------------;
CF_BUF: .DS  512

;-------------------------------------------------------------------------------;
; Messages to print to console                                                  ;
;-------------------------------------------------------------------------------;
BIOS_TEXT:
        .DB  $0D,$0A
        .DB  "Starting BIOS..."
        .DB  $0D,$0A,$00

CPM_LOADING_TEXT:
        .DB  $0D,$0A
        .DB  "Loading CP/M..."
        .DB  $0D,$0A,$00

CPM_STARTING_TEXT:
        .DB  $0D,$0A
        .DB  "CP/M 2.2"
        .DB  $0D,$0A
        .DB  "Copyright 1979 (c) by Digital Research"
        .DB  $0D,$0A,$00

BUFFER_OVERRUN_TEXT:
        .DB  $0D,$0A
        .DB  "WARNING: Keyboard Buffer Overrun"
        .DB  $0D,$0A,$00

;-------------------------------------------------------------------------------;
; IDE (Compact Flash) error messages                                            ;
;-------------------------------------------------------------------------------;
IDE_ERROR_MESSAGES:       
        .DB "DAM not found          ",$0D,$0A,$00
        .DB "Track 0 not found      ",$0D,$0A,$00
        .DB "Command aborted        ",$0D,$0A,$00
        .DB "Unknown error          ",$0D,$0A,$00
        .DB "ID not found           ",$0D,$0A,$00
        .DB "Unknown error          ",$0D,$0A,$00
        .DB "Uncorrectable ECC error",$0D,$0A,$00
        .DB "Bad block detected     ",$0D,$0A,$00

;-------------------------------------------------------------------------------;
; Interupt Mode 2 lookup for serial interrupt (must be at page $FF)             ;
;-------------------------------------------------------------------------------;
        .ORG $FF80
IO_INT_HAND_PTR:     
        .DW  IO_INTERRUPT_HANDLER

;-------------------------------------------------------------------------------;
; Stack area                                                                    ;
;-------------------------------------------------------------------------------;
        .DS  $7E                        ; Stack starts at the top of the memory 
STACK:  .EQU $FFFF                      ; in the BIOS RAM area going down 

.END    


