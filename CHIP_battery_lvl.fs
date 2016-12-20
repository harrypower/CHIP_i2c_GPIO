\ This Gforth code reads the battery level on C.H.I.P. hardware in Gforth
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

\  I have used information from /usr/bin/battery.sh found on headless chip os 4.4

require CHIP_Gforth_i2c.fs
require script.fs

variable voltage-lsb
variable voltage-msb
variable mybuffer

cr
." This is the data recieved with the bash method:"
s" i2cset -y -f 0 0x34 0x82 0xC3" system
s" i2cget -y -f 0 0x34 0x78" sh-get type
s" i2cget -y -f 0 0x34 0x79" sh-get type
cr

: battery-read-prep ( -- )
  0 0x34 1 CHIPi2copen dup { handle } true = throw
  0x82 mybuffer c! 0xc3 mybuffer 1 + c!
  handle mybuffer 2 CHIPi2cwrite 0 <= throw
  handle CHIPi2cclose ;

: read-msb ( -- )
  0 0x34 1 CHIPi2copen dup { handle } true = throw
  handle 0x78 CHIPi2cwrite-b throw
  0 0x34 1 CHIPi2copen dup to handle true = throw
  handle voltage-msb CHIPi2cread-b throw
  handle CHIPi2cclose ;

: read-lsb ( -- )
  0 0x34 1 CHIPi2copen dup { handle } true = throw
  handle 0x79 CHIPi2cwrite-b throw
  0 0x34 1 CHIPi2copen dup to handle true = throw
  handle voltage-lsb CHIPi2cread-b throw
  handle CHIPi2cclose ;

: read-battery ( -- )
  battery-read-prep
  read-msb
  read-lsb
  voltage-msb @ 4 lshift
  voltage-lsb @ 0x0f and or s>f 1.1e f*
  ." Chip battery voltage is " f. ." mv" cr ;

read-battery
voltage-msb 4 dump
voltage-lsb 4 dump
." This data should match data from the above bash data!" cr
