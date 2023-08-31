ENTRY EQU $0005

ORG A, RTEST.COM

FCB     .EQU    $5C             ; Default file control block

        LD      DE,FCB          ; Set parameter to FCB = (file)argument from command line
        LD      C,15            ; Routine to open a file 
        CALL    ENTRY           ; (DE) points to the FCB
        CP      $FF             ; Check for error code
        JP      Z,ERROR1        ; Open error ?
        LD      B,4             ; B = number of (128 byte) sectors to read
        LD      HL,DATA         ; Copy to here

LOAD1:  PUSH    BC              ; Save counter
        PUSH    HL              ; Save pointer to DATA
        EX      DE,HL           ; Set parameter to DATA
        LD      C,26            ; Routine to set the DMA address to (DE)
        CALL    ENTRY           
        LD      DE,FCB          ; Set parameter to FCB = (file)argument from command line
        LD      C,20            ; Routine to read the next record of a sequential file 
        CALL    ENTRY           ; (DE) points to the FCB
        OR      A               ; Set zero flag if appropriate
        JP      NZ,ERROR2       ; Read error ?
        POP     HL        
        LD      A,128
        ADD     A,L
        LD      L,A        
        JP      NC,LOAD2
        INC     H
LOAD2:  POP     BC
        DEC     B
        JP      NZ,LOAD1        ; Repeat until all sectors read

PROCESS:
        LD      DE,$40          ; Just write 64 bytes from this file
        LD      HL,DATA

PROCESS1:
        PUSH    DE
        PUSH    HL
        LD      A,(HL)
        CALL    BYTE2HEX        ; Convert register A to ASCII representation of hexadecimal number
        LD      E,B             ; Result is in BC, so print in order
        LD      D,C             
        LD      C,2
        CALL    ENTRY
        LD      E,D
        LD      C,2
        CALL    ENTRY
        LD      E,' '           ; Print seperator
        LD      C,2
        CALL    ENTRY
        POP     HL
        POP     DE
        INC     HL
        DEC     DE
        LD      A,D             ; Done if register D is 0
        CP      $00
        JR      NZ, PROCESS1
        LD      A,E             ; And register E is 0
        CP      $00
        JR      NZ, PROCESS1

; Get here after reading all of the file
FINISHED:
        LD      HL,OK_MSG       ; OK message to console AND return
        CALL    PLINE
        RET

; Binary to Hexadecimal Ascii
;        input : A 
;        return: BC
BYTE2HEX:
        LD      C,A             
        AND     $0F             ; Get low nibble
        CALL    NIBBLE2HEX      ; Convert
        LD      A,C 
        LD      C,B             ; Result of low nibble to C 
        AND     $F0             ; Get high nibble
        SRL     A               ; Shift right     
        SRL     A               ; Shift right     
        SRL     A               ; Shift right     
        SRL     A               ; Shift right     

NIBBLE2HEX:                     ; A holds nibble
        ADD     A,$90
        DAA
        ADC     A,$40
        DAA                     
        LD      B,A             ; B holds hex ASCII        
        RET
        
; Print out error message
ERROR1: LD      HL,ERROR_MSG1
        CALL    PLINE
        RET

ERROR2: POP     HL
        POP     BC
        LD      HL,ERROR_MSG2
        CALL    PLINE
        RET

; Routine to print a character string pointed to be (HL) on the console
; It must terminated with a null byte
PLINE:  LD      A,(HL)
        OR      A
        RET     Z
        INC     HL
        PUSH    HL
        LD      E,A             ; Char to be printed in E
        LD      C,2             ; Routine to print (E) to the console
        CALL    ENTRY
        POP     HL
        JP      PLINE

OK_MSG:
        .DB  $0D,$0A
        .DB  "Done" 
        .DB  $0D,$0A,$00

ERROR_MSG1:
        .DB  $0D,$0A
        .DB  "Error opening file"
        .DB  $0D,$0A,$00

ERROR_MSG2:
        .DB  $0D,$0A
        .DB  "Error reading data from device"
        .DB  $0D,$0A,$00

DATA:   .DS  512 

        END
