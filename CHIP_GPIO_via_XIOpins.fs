\ This Gforth code does GPIO at the XIO pins via the PCF8574A expander device on CHIP
\    Copyright (C) 2016  Philip K. Smith

\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.

\  The PCF8574A device is an i2c device at address 0x38 and i2c-2 address on the CHIP board
\  This code simply forces access to that device to allow reading and writing a byte at a time

require ./CHIP_Gforth_i2c.fs

variable xio_buffer

\ note these low level read and write words assume the PCF8574A device is at i2c-2 port and address 0x38
\ this is directly from the CHIP schematics and is confirmed by other code snipets i have seen.
\ It works but if this expander is moved then this code would need to be updated for this!
\ The data read or writen is always all 8 pins at the same time so binary mask should be used if you just want some pins!

: readxio ( -- uc nflag ) \ returns uc value a byte from the PCF8574A expander 8 pins
  \ uc is valid if nflag is false.  us is not valid if nflag is true
  try
    2 0x38 1 CHIPi2copen dup { xiohandle } true = if
      true throw
    else
      xiohandle xio_buffer CHIPi2cread-b throw
      xiohandle CHIPi2cclose throw
    then
    false
  restore
    if 0 true else xio_buffer @ false then
  endtry ;

: writexio ( uc -- nflag ) \ writes uc byte to the PCF8574A expander 8 pins
  \ uc is the byte to write! nflag is false if no errors occured during writing or true for some error
  try
    2 0x38 1 CHIPi2copen dup { xiohandle } true = if
      true throw
    else
      xiohandle swap CHIPi2cwrite-b throw
      xiohandle CHIPi2cclose throw
    then
    false
  restore dup if swap drop then
  endtry ;

: fastwritexio ( caddr uqnt -- usent nflag ) \ write a stream of bytes to the xio expander
  \ caddr is the address where the bytes start and uqnt is the amount of bytes to write
  \ usent will indicate how many of the uqnt bytes sent if nflag is false
  \ if nflag is true then usent is not valid
  try
    2 0x38 1 CHIPi2copen dup { xiohandle } true = if
      true throw
    else
      xiohandle rot rot  CHIPi2cwrite dup true = if throw then
      xiohandle CHIPi2cclose throw
    then
    false
  restore dup if rot drop then
  endtry ;

\ \\\ example of use

: thousand-slow ( -- ) \ simply put out a pulse train of 1000 1 and 0's on pin XIO-P0
  1000 0 do 1 writexio drop 0 writexio drop loop ;

create write-data
\ put data into write-data storage for pulse train
1000 0 [do] 1 c, 0 c, [loop]

: thousand-fast ( -- ) \ faster pulse train of 1000 1 and 0's on pin XIO-P0
  write-data 2000 fastwritexio drop drop ;

: read-write-xio-pins ( -- uxio-p3 nflag ) \ read xio-p3 and write the data to xio-p2
  \ uxio-p3 is the value of p3 at the time of reading it in this code
  \ uxio-p3 data is valid if nflag is false and not valid if nflag is true
  \ nflag is true if any part of the read write open close operations failed and false if read and write worked properly.
  try
    %1000 writexio throw 
    readxio throw
    %1000 and \ mask out only xio-p3 and copy it for output
    1 rshift \ shift xio-p3 data to xio-p2 for writing
    dup 3 rshift \ shift uxio-p3 data to show as a 0 or 1
    swap writexio throw \ write the xio-p3 data out to xio-p2 pin
    false
  restore dup if 0 true then \ clean up stack it this code has thrown any errors
  endtry ;
