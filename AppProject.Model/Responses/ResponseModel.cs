namespace AppProject.Model.Responses;

public class ResponseModel
{
    public static MessageModel<T> Success<T>(T data, string msg = "成功")
    {
        return new MessageModel<T>
        {
            success = true,
            msg = msg,
            response = data
        };
    }

    public static MessageModel Success(string msg = "成功")
    {
        return new MessageModel
        {
            success = true,
            msg = msg
        };
    }
    
    public static MessageModel Failed(string msg = "失败", int status = 500)
    {
        return new MessageModel()
        {
            success = false,
            status = status,
            msg = msg
        };
    }
    
    public static MessageModel<T> Failed<T>(string msg = "失败", int status = 500)
    {
        return new MessageModel<T>()
        {
            success = false,
            status = status,
            msg = msg,
            response = default
        };
    }
}