ENTRY EQU $0005

ORG A, WTEST.COM

        LD      DE,FCB          ; Set parameter to FCB 
        LD      C,15            ; Routine to open a file 
        CALL    ENTRY           ; (DE) points to the FCB
        CP      $FF             ; Check for error code, if file exists (no error returned) give error message
        JP      NZ,ERROR1       ; No open error ? Goto error
        LD      DE,FCB          ; Set parameter to FCB 
        LD      C,22            ; Routine to create the file pointed to by (DE)
        CALL    ENTRY           
        INC     A               ; Set zero if 0xFF returned.
        JP      Z,ERROR2        ; Can't create ? Goto error
        
        LD      B,4             ; B = number of (128 byte) sectors to write
        LD      HL,DATA         ; Copy from here

SAVE1:  PUSH    BC              ; Save counter
        PUSH    HL              ; Save pointer to DATA
        EX      DE,HL           ; Set parameter to DATA
        LD      C,26            ; Routine to set the dma address to (DE)
        CALL    ENTRY           
        LD      DE,FCB          ; Set parameter to FCB
        LD      C,21            ; Routine to write the next record of a sequential file 
        CALL    ENTRY           ; (DE) points to the FCB
        OR      A               ; Set zero flag if appropriate
        JP      NZ,ERROR3       ; Write error ?
        POP     HL        
        LD      A,128
        ADD     A,L
        LD      L,A        
        JP      NC,SAVE2
        INC     H
SAVE2:  POP     BC
        DEC     B
        JP      NZ,SAVE1        ; Repeat until all sectors written

; Get here after writing all of the file
FINISHED:
        LD      DE,FCB          ; Set parameter to FCB
        LD      C,16            ; Routine to close a file, (DE) points to the FCB 
        CALL    ENTRY        
        INC     A               ; Did it close ok ?
        JP      Z,ERROR4        ; Can't close ? Goto error
        LD      HL,OK_MSG       ; OK message to console and return
        CALL    PLINE
        RET

; Print out error message
ERROR1: LD      HL,ERROR_MSG1
        CALL    PLINE
        RET

ERROR2: LD      HL,ERROR_MSG2
        CALL    PLINE
        RET

ERROR3: POP     HL
        POP     BC
        LD      HL,ERROR_MSG3
        CALL    PLINE
        RET

ERROR4: LD      HL,ERROR_MSG4
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
        .DB  "File 'TEST.TXT' created" 
        .DB  $0D,$0A,$00

ERROR_MSG1:
        .DB  $0D,$0A
        .DB  "File already exists"
        .DB  $0D,$0A,$00

ERROR_MSG2:
        .DB  $0D,$0A
        .DB  "Error creating file"
        .DB  $0D,$0A,$00

ERROR_MSG3:
        .DB  $0D,$0A
        .DB  "Error writing data to device"
        .DB  $0D,$0A,$00

ERROR_MSG4:
        .DB  $0D,$0A
        .DB  "Error closing file"
        .DB  $0D,$0A,$00

DATA:   .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "################"
        .DB  "@@@@@@@@@@@@@@@@"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"
        .DB  "$$$$$$$$$$$$$$$$"
        .DB  "****************"

; File control block setup 
FCB:    .DB     0
        .TEXT   "TEST    TXT"
        .DB     0,0,0,0,0
        .TEXT   "           "
        .DB     0,0,0,0,0

