Release Notes for PICkitminus Microcontroller Programmer
PICkitminus V3.27.00
Device File V2.64.28


KNOWN ISSUES / LIMITATIONS
- Programmer-to-Go not supported for PIC24/dsPIC33 parts with
  Customer OTP memory
- Programmer-to-Go with PICkit2 on PIC18F_K90_K80_K22 family
  requires PICkit2 operating system 2.32.01. It is not automatically
  updated if user wants to retain the last official PICkit2
  operating system 2.32.00. Update manually from Tools menu if needed
- Programming of parts with dual partition flash is supported
  only in single partition mode
- OTP config registers in PIC18F MSB1st parts are not written
  to prevent accidental write. Please contact me if you need this
  feature