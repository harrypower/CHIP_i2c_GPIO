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

create voltage-lsb 2 allot
create voltage-msb 2 allot

: battery-voltage-read ( -- )
  \ look at bash code and see the write that happens first
  \ add this write here then test
  0 0x34 CHIPi2copen dup { handle }  0 <= throw
  handle voltage-msb 0x78 CHIPi2cread-b throw
  handle voltage-lsb 0x79 CHIPi2cread-b throw
  handle CHIPi2cclose  ;

battery-voltage-read
voltage-lsb c@ . ." voltage-lsb" cr
voltage-msb c@ . ." voltage-msb" cr
