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
