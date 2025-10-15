15. Oct 2025
Release Notes for PICkitminus Microcontroller Programmer
PICkitminus V3.28.03
Device File V2.64.36

Number of supported devices: 1594

KNOWN ISSUES / LIMITATIONS
- Programmer-to-Go not supported for PIC24/dsPIC33 parts with
  Customer OTP memory
- Programming of parts with dual partition flash is supported
  only in single partition mode
- OTP config registers in PIC18F MSB1st parts are not written
  to prevent accidental write. Please contact me if you need this
  feature