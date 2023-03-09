# Log4WCL
Tool for WCL Log Analyzing

## provide
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
