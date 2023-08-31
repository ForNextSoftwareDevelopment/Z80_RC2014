;
;  Compute a Mandelbrot set on a simple Z80 computer.
;
; From https://rosettacode.org/wiki/Mandelbrot_set#Z80_Assembly
; Adapted to CP/M and colorzied by J.B. Langston
; Latest version at https://gist.github.com/jblang/3b17598ccfa0f7e5cca79ad826a399a9
; Assemble with sjasm
;
; Porting this program to another Z80 platform should be easy and straight-
; forward: The only dependencies on my homebrew machine are the system-calls
; used to print strings and characters. These calls are performed by loading
; IX with the number of the system-call and performing an RST 08. To port this
; program to another operating system just replace these system-calls with
; the appropriate versions. Only three system-calls are used in the following:
; _crlf: Prints a CR/LF, _puts: Prints a 0-terminated string (the adress of
; which is expected in HL), and _putc: Print a single character which is
; expected in A. RST 0 give control back to the monitor.
;

org A, MANDEL.COM
                
                jp      start
                
bdos            equ     05h                     ; bdos vector
conout          equ     2                       ; console output bdos call
prints          equ     9                       ; print string bdos call
cr              equ     13                      ; carriage return
lf              equ     10                      ; line feed
esc             equ     27                      ; escape
eos             equ     '$'                     ; end of string marker
pixel           equ     219                     ; character to output for pixel
scale           equ     256                     ; Do NOT change this - the
                                                ; arithmetic routines rely on
                                                ; this scaling factor! :-)
divergent       equ     scale * 4

iteration_max:  defb    30                      ; How many iterations
x:              defw    0                       ; x-coordinate
x_start:        defw    -2 * scale              ; Minimum x-coordinate
x_end:          defw    1 * scale               ; Maximum x-coordinate
x_step:         defw    scale / 80              ; x-coordinate step-width
y:              defw    -5 * scale / 4          ; Minimum y-coordinate
y_end:          defw    5 * scale / 4           ; Maximum y-coordinate
y_step:         defw    scale / 60              ; y-coordinate step-width
z_0:            defw    0
z_1:            defw    0
scratch_0:      defw    0
z_0_square_high:defw    0
z_0_square_low: defw    0
z_1_square_high:defw    0
z_1_square_low: defw    0
display:        defb    " .-+*=#@"              ; 8 characters for the display

hsv:            defb 0                          ; hsv color table
                defb 201, 200, 199, 198, 197
                defb 196, 202, 208, 214, 220
                defb 226, 190, 154, 118, 82
                defb 46, 47, 48, 49, 50
                defb 51, 45, 39, 33, 27
                defb 21, 57, 93, 129, 165

welcome:        defb    "Generating a Mandelbrot set"
crlf:           defb    cr, lf, eos
finished:       defb    esc, "[0mComputation finished.", cr, lf, eos
ansifg:         defb    esc, "[38;5;", eos
ansibg:         defb    esc, "[48;5;", eos


start:          ld      de, welcome             ; Print a welcome message
                ld      c, prints
                call    bdos

; for (y = <initial_value> ; y <= y_end; y += y_step)
; {
outer_loop:     ld      hl, (y_end)             ; Is y <= y_end?
                ld      de, (y)
                and     a                       ; Clear carry
                sbc     hl, de                  ; Perform the comparison
                jp      m, mandel_end           ; End of outer loop reached

;    for (x = x_start; x <= x_end; x += x_step)
;    {
                ld      hl, (x_start)           ; x = x_start
                ld      (x), hl
inner_loop:     ld      hl, (x_end)             ; Is x <= x_end?
                ld      de, (x)
                and     a
                sbc     hl, de
                jp      m, inner_loop_end       ; End of inner loop reached

;      z_0 = z_1 = 0;
                ld      hl, 0
                ld      (z_0), hl
                ld      (z_1), hl

;      for (iteration = iteration_max; iteration; iteration--)
;      {
                ld      a, (iteration_max)
                ld      b, a
iteration_loop: push    bc                      ; iteration -> stack
;        z2 = (z_0 * z_0 - z_1 * z_1) / SCALE;
                ld      de, (z_1)               ; Compute DE HL = z_1 * z_1
                ld      b, d
                ld      c, e
                call    mul_16
                ld      (z_0_square_low), hl    ; z_0 ** 2 is needed later again
                ld      (z_0_square_high), de

                ld      de, (z_0)               ; Compute DE HL = z_0 * z_0
                ld      b, d
                ld      c, e
                call    mul_16
                ld      (z_1_square_low), hl    ; z_1 ** 2 will be also needed
                ld      (z_1_square_high), de

                and     a                       ; Compute subtraction
                ld      bc, (z_0_square_low)
                sbc     hl, bc
                ld      (scratch_0), hl         ; Save lower 16 bit of result
                ld      h, d
                ld      l, e
                ld      bc, (z_0_square_high)
                sbc     hl, bc
                ld      bc, (scratch_0)         ; HL BC = z_0 ** 2 - z_1 ** 2

                ld      c, b                    ; Divide by scale = 256
                ld      b, l                    ; Discard the rest
                push    bc                      ; We need BC later

;        z3 = 2 * z0 * z1 / SCALE;
                ld      hl, (z_0)               ; Compute DE HL = 2 * z_0 * z_1
                add     hl, hl
                ld      d, h
                ld      e, l
                ld      bc, (z_1)
                call    mul_16

                ld      b, e                    ; Divide by scale (= 256)
                ld      c, h                    ; BC contains now z_3

;        z1 = z3 + y;
                ld      hl, (y)
                add     hl, bc
                ld      (z_1), hl

;        z_0 = z_2 + x;
                pop     bc                      ; Here BC is needed again :-)
                ld      hl, (x)
                add     hl, bc
                ld      (z_0), hl

;        if (z0 * z0 / SCALE + z1 * z1 / SCALE > 4 * SCALE)
                ld      hl, (z_0_square_low)    ; Use the squares computed
                ld      de, (z_1_square_low)    ; above
                add     hl, de
                ld      b, h                  ; BC contains lower word of sum
                ld      c, l

                ld      hl, (z_0_square_high)
                ld      de, (z_1_square_high)
                adc     hl, de

                ld      h, l                    ; HL now contains (z_0 ** 2 +
                ld      l, b                    ; z_1 ** 2) / scale

                ld      bc, divergent
                and     a
                sbc     hl, bc

;          break;
                jp      c, iteration_dec        ; No break
                pop     bc                      ; Get latest iteration counter
                jr      iteration_end           ; Exit loop

;        iteration++;
iteration_dec:  pop     bc                      ; Get iteration counter
                djnz    iteration_loop          ; We might fall through!
;      }
iteration_end:
;      printf("%c", display[iteration % 7]);
                call    colorpixel
                ld      c, conout               ; Print the character
                call    bdos

                ld      de, (x_step)            ; x += x_step
                ld      hl, (x)
                add     hl, de
                ld      (x), hl

                jp      inner_loop
;    }
;    printf("\n");
inner_loop_end:
                ld      de, crlf
                ld      c, prints              ; Print a CR/LF pair
                call    bdos

                ld      de, (y_step)            ; y += y_step
                ld      hl, (y)
                add     hl, de
                ld      (y), hl                 ; Store new y-value

                jp      outer_loop
; }

mandel_end:     ld      de, finished            ; Print finished-message
                ld      c, prints
                call    bdos

                ret                                 ; Return to CP/M
                
colorpixel:     ld      c,b                     ; iter count in BC
                ld      b,0
                ld      hl, hsv                 ; get ANSI color code
                add     hl, bc
                ld      a,(hl)
                call    setcolor
                ld      e, pixel                ; show pixel
                ret
                
asciipixel:     ld      a, b
                and     $7                      ; lower three bits only (c = 0)
                sbc     hl, hl
                ld      l, a
                ld      de, display             ; Get start of character array
                add     hl, de                  ; address and load the
                ld      e, (hl)                 ; character to be printed
                ret

setcolor:       push    af                      ; save accumulator
                ld      de,ansifg               ; start ANSI control sequence
                ld      c,prints                ; to set foreground color
                call    bdos
                pop     af
                call    printdec                ; print ANSI color code
                ld      c,conout
                ld      e,'m'                   ; finish control sequence
                call    bdos
                ret
                
printdec:       ld      c,-100                  ; print 100s place
                call    pd1
                ld      c,-10                   ; 10s place
                call    pd1
                ld      c,-1                    ; 1s place
pd1:            ld      e,'0'-1                 ; start ASCII right before 0
pd2:            inc     e                       ; increment ASCII code
                add     a,c                     ; subtract 1 place value
                jr      c,pd2                   ; loop until negative
                sub     c                       ; add back the last value
                push    af                      ; save accumulator
                ld      a,-1                    ; are we in the ones place?
                cp      c
                jr      z,pd3                   ; if so, skip to output
                ld      a,'0'                   ; don't print leading 0s
                cp      e
                jr      z,pd4
pd3:            ld      c,conout
                call    bdos
pd4:            pop     af                      ; restore accumulator
                ret


;
;   Compute DEHL = BC * DE (signed): This routine is not too clever but it
; works. It is based on a standard 16-by-16 multiplication routine for unsigned
; integers. At the beginning the sign of the result is determined based on the
; signs of the operands which are negated if necessary. Then the unsigned
; multiplication takes place, followed by negating the result if necessary.
;
mul_16:         xor     a                       ; Clear carry and A (-> +)
                bit     7, b                    ; Is BC negative?
                jr      z, bc_positive          ; No
                sub     c                       ; A is still zero, complement
                ld      c, a
                ld      a, 0
                sbc     a, b
                ld      b, a
                scf                             ; Set carry (-> -)
bc_positive:    bit     7, D                    ; Is DE negative?
                jr      z, de_positive          ; No
                push    af                      ; Remember carry for later!
                xor     a
                sub     e
                ld      e, a
                ld      a, 0
                sbc     a, d
                ld      d, a
                pop     af                      ; Restore carry for complement
                ccf                             ; Complement Carry (-> +/-?)
de_positive:    push    af                      ; Remember state of carry
                and     a                       ; Start multiplication
                sbc     hl, hl
                ld      a, 16                   ; 16 rounds
mul_16_loop:    add     hl, hl
                rl      e
                rl      d
                jr      nc, mul_16_exit
                add     hl, bc
                jr      nc, mul_16_exit
                inc     de
mul_16_exit:    dec     a
                jr      nz, mul_16_loop
                pop     af                      ; Restore carry from beginning
                ret     nc                      ; No sign inversion necessary
                xor     a                       ; Complement DE HL
                sub     l
                ld      l, a
                ld      a, 0
                sbc     a, h
                ld      h, a
                ld      a, 0
                sbc     a, e
                ld      e, a
                ld      a, 0
                sbc     a, d
                ld      d, a
                ret