# Log4WCL
Tool for WCL Log Analyzing

## Provide
#### Log split
```c#
WCLParser p = new WCLParser();
var files = p.SplitFiles(your_wcl_log_path);
```

#### Log Analyze
``` c#
WCLParser p = new WCLParser();
var files = p.SplitFiles("your_wcl_log_path");
foreach (var file in files)
{
  p.Clear();
  p.Parse("your_wcl_log_path" + file);
}
```
#### Log Modify
``` c#
 WCLParser p = new WCLParser();
 p.Parse("D:/4.txt");
WCLLogger.LogDPS(WCLParser.Events);
WCLModifierDamage mod = new WCLModifierDamage();
mod.DamageSrcSkillName = new string[] { "\"内部腐烂\"" };
mod.DamageDstRoleName = new string[] { "\"再见幼儿园-銀翼要塞\"" };
mod.DamageDstSkillName = new string[] { "\"剑刃乱舞\"" };
mod.Value = 1000;
mod.Run();
```
