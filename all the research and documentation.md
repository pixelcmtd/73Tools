# Tha log naming
The raw logs are always about 1 MiB big.
Their names are always in the following format:
$"SADLog bis {mm}-{dd}-{yyyy} {hh}-{mm}-{ss}.txt",
except the newest one, which is not finished and called "SADLog.txt".

# Tha line syntax
Every line of log is one event in the following format:
$"{dd}.{mm}.{yyyy}	{hh}:{mm}:{ss}	{action} in {caller}	tokens"
(yes, these are actual tabulators)

# Tha actions
Known action names(they will be documented later):
StartProgram
OnReceive
NachrichtBehandeln
Senden
SetNTFSBerechtigungen
AddNewSession
Error
RemoveSession
MainSAD
CheckBasisserver
ConnectDB
IsPC
GetMandant
GetServerName
LesePfade
SALMVerbinden
NachrichtSenden
NachrichtErhalten
UnbeaufsichtigterModus
In (this might only exist due to a parsing error)
GetClientIPAddress

# Tha 73XML-Format
The 73XML-Format is a notation of 73-logs which is designed to be both valid XML and human readable.
Encoding a single line in C# looks like this:
    $"<sad><line name=\"{name}\" caller=\"{caller}\" tokens=\"{tokens}\" /></sad>"

The 73DB-Format:
```cpp
#include <stdint.h>
#include <string>
using namespace std;
struct line
{
	uint8_t name_length;
	string name;
	uint8_t caller_length;
	string caller;
	int64_t timestamp;
	uint16_t token_length;
	string tokens;
}
```
73DB: Deflate{line}
