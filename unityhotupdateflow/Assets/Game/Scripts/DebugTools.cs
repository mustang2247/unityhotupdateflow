using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 日志打印类
/// </summary>
public class DebugTools
{
    /**
     * show waring log
     * 显示警告日志
     *
     * @param object cMessage
     * @return void
     */
    public static void LogWarning(object cMessage)
    {
#if UNITY_EDITOR
        Debug.LogWarning(cMessage);
#else
        
#endif
    }

    /**
     * show error log
     * 显示错误日志
     * 
     * @param object cMessage
     * @return void
     */
    public static void LogError(object cMessage)
    {
#if UNITY_EDITOR
        Debug.LogError(cMessage);
#else
        
#endif
    }

    /**
    * show log
    * 显示普通日志
    * 
    * @param object cMessage
    * @return void
    */
    public static void TestLog(object cMessage)
    {

#if UNITY_EDITOR
        Debug.Log(cMessage);
#else
        Debug.Log(cMessage);
#endif
    }

    /**
     * show log
     * 显示普通日志
     * 
     * @param object cMessage
     * @return void
     */
    public static void Log(object cMessage)
    {

#if UNITY_EDITOR
        Debug.Log(cMessage);
#else
        //Debug.Log(cMessage);
#endif
    }

    /**
     * show exception log
     * 显示异常日志
     * 
     * @param Exception cMessage
     * @return void
     */
    public static void LogException(Exception cMessage)
    {
#if UNITY_EDITOR
        Debug.LogException(cMessage);
#else
        
#endif
    }
}
