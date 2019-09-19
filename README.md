## SkyrimLib

Extremely rudimentary library that can be used to quickly easily parse a Skyrim (or Skyrim Special Edition) ESP file (including ESM and ESL). Writing is implemented but do not expect to get back a file that the game will accept.

##### TODO (in order of importance):
- Figure out a way to handle XXXX subrecords when writing the data back to disk.
- Write definition files for all record types and generate record and subrecord wrappers for them.
- Handle BSA files (using both ZLib and LZ4 compression).
- Write actual documentation.

[UESP](https://en.uesp.net/wiki/Skyrim) was used as a reference for the file formats. 

##### WARNING
The API is not currently set in stone and will most probably change a lot in the near future.

##### Credits
[Noggog](https://github.com/Noggog) for his invaluable advice so far.

##### Disclaimer
If anyone recognizes any code and I've not mentioned them in the credits, contact me (with some proof obviously) and I will either credit you or re-write said code.