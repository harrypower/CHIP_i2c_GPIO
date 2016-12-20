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
variable buffer

s" i2cset -y -f 0 0x34 0x82 0xC3" system
s" i2cget -y -f 0 0x34 0x78" sh-get type
s" i2cget -y -f 0 0x34 0x79" sh-get type

\\\
: battery-voltage-read ( -- )
  0 0x34 CHIPi2copen dup { handle } true = throw
  buffer 0xc3 c!
  handle 0x82 buffer 1 CHIPwrite-ign-nack 0 <= throw
  handle CHIPi2cclose

  0 0x34 CHIPi2copen dup to handle true = throw
  handle 0x78 voltage-msb 1 CHIPread-no-ack 0 <= throw
  handle CHIPi2cclose

  0 0x34 CHIPi2copen dup to handle true = throw
  handle 0x79 voltage-lsb 1 CHIPread-no-ack 0 <= throw
  handle CHIPi2cclose  ;

battery-voltage-read
voltage-lsb c@ . ." voltage-lsb" cr
voltage-msb c@ . ." voltage-msb" cr

voltage-msb 4 dump cr
voltage-lsb 4 dump cr
