using System;
public class MyProcessSendObject : MarshalByRefObject
{
    private string taskInfo = string.Empty;

    public void Add(string taskInfo)
    {
        Console.WriteLine("Add：{0}", taskInfo);
        this.taskInfo = taskInfo;
    }

    public string GetTask()
    {
        Console.WriteLine("GetTask：{0}", taskInfo);
        return taskInfo;
    }

}