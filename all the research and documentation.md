# Tha log naming
The raw logs are always about 1 MiB big.  
Their names are always in the following format:  
```cs
$"SADLog bis {mm}-{dd}-{yyyy} {hh}-{mm}-{ss}.txt"
```
except the newest one, which is not finished and called "SADLog.txt".  

# Tha line syntax
Almost every line of log is one event in the following format:  
```cs
$"{dd}.{mm}.{yyyy}	{hh}:{mm}:{ss}	{action} in {caller}	{tokens}	{error}"
```
(yes, these are actual tabulators)

# Tha known action names(they will be documented later)  
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
IsPC (exception to the normal line syntax)  
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
```cs
$"<sad><line name=\"{name}\" caller=\"{caller}\" tokens=\"{tokens}\" error=\"{error}\" /></sad>"
```

# Tha 73DB-Format
The 73DataBase-Format is a Deflate wrapper around line objects.  
The sequentially saved line objects look like this:
```c
struct line
{
	uint8_t  name_length; //byte count
	utf8     name[name_length];
	uint8_t  caller_length; //byte count
	utf8     caller[caller_length];
	int64_t  timestamp; //little endian
	uint16_t token_length; //little endian
	utf8     tokens;
	int32_t  error; //little endian
}
```
The line objects are all just appended and compressed with Deflate.
