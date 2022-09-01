using System;
public class MyProcessSendObject : MarshalByRefObject
{
    private string taskInfo = string.Empty;
    public delegate void StartRecordEventHandler(string fileInfo);
    public static event StartRecordEventHandler StartRecord;

    public delegate void StopRecordEventHandler();
    public static event StopRecordEventHandler StopRecord;
    public void Add(string taskInfo)
    {
        //Console.WriteLine("Add：{0}", taskInfo);
        //this.taskInfo = taskInfo;
        string[] content = taskInfo.Split('|');
        if (content[0] == "StartRecord")
        {
            StartRecord(content[1]);
        }
        else if (content[0] == "StopRecord")
        {
            StopRecord();
        }
    }

    public string GetTask()
    {
        //Console.WriteLine("GetTask：{0}", taskInfo);
        return taskInfo;
    }
}