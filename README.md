# todo-winforms（墨迹清单 · WinForms）

与 [todo-lazarus](../todo-lazarus) / [moji-qingdan](../moji-qingdan) 同功能的 **C# WinForms** 待办清单。

## 构建

```bash
dotnet build -c Release
```

输出：`bin/Release/net8.0-windows/墨迹清单.exe`

## 功能

- 添加、勾选完成、双击编辑、删除、上移/下移
- 筛选：全部 / 未完成 / 已完成
- 导出 / 导入 Markdown；JSON 导入覆盖本地库；粘贴导入
- 数据：`%APPDATA%\todo-winforms\todos.json`
