# CloudFileBackup
Background service to sync files.

Simple worker for syncing specified watch directories to another directory. For example, if you're company uses network drives for users, and you need to have 
backups for your source code, but aren't ready to commit certain changes. Each WatchDirectory needs a BackupDirectory assigned at the same index, i.e. 
```javascript
"WatchDirectories": [
  "your/local/directory/to/backup/watch"
  ],
  "BackupDirectories": [
  "your/cloud/directory/to/backup/watch" 
  ]
  ```
  Set ExecutionInterval to desired time in milliseconds. Backups will be ran once every interval set, and in parallel for every specified watch directory. 
  You can easily configure this to your advantage to use the amount of parallelism you desire.
