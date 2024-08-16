using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ProjectBase
{
    public class BinaryMgr : BaseManager<BinaryMgr>
    {

        public static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/Binary/";

        //string是数据结构类的名字，object是Container
        public static Dictionary<string, object> tableDic = new Dictionary<string, object>();


        /// <summary>
        /// 读取二进制Binary文件的数据
        /// </summary>
        /// <typeparam name="T">容器类</typeparam>
        /// <typeparam name="K">数据类</typeparam>
        public void LoadTable<T, K>()
        {
            using (FileStream fs = File.Open(DATA_BINARY_PATH + typeof(K).Name + ".zhou", FileMode.Open, FileAccess.Read))
            {
                //读取文件的内容到data中
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                //读取指针
                int index = 0;

                //读取row的数量
                int rowCount = BitConverter.ToInt32(data, index);
                index += 4;

                //获取主键名字长度和内容
                int keyNameLength = BitConverter.ToInt32(data, index);
                index += 4;
                string keyName = Encoding.UTF8.GetString(data, index, keyNameLength);
                index += keyNameLength;

                //创建容器类对象
                Type containerType = typeof(T);
                object containerObj = Activator.CreateInstance(containerType);
                
                Type classType = typeof(K);

                //获取数据结构类所有 字段的信息
                FieldInfo[] infos = classType.GetFields();

                //开始读取具体每一行的信息
                for(int i = 0; i < rowCount; i++)
                {
                    object classObj = Activator.CreateInstance(classType);
                    foreach(FieldInfo info in infos)
                    {
                        if(info.FieldType == typeof(int))
                        {
                            info.SetValue(classObj, BitConverter.ToInt32(data, index));
                            index += 4;
                        }else if (info.FieldType == typeof(float))
                        {
                            info.SetValue(classObj, BitConverter.ToSingle(data, index));
                            index += 4;
                        }
                        else if (info.FieldType == typeof(bool))
                        {
                            info.SetValue(classObj, BitConverter.ToBoolean(data, index));
                            index += 1;
                        }
                        else if (info.FieldType == typeof(string))
                        {
                            int length = BitConverter.ToInt32(data, index);
                            index += 4;
                            info.SetValue(classObj, Encoding.UTF8.GetString(data, index, length));
                            index += length;
                        }
                    }

                    //读取完 一行数据 把数据加入到容器中
                    object dicObject = containerType.GetField("dataDic").GetValue(containerObj);

                    MethodInfo mInfo = dicObject.GetType().GetMethod("Add");

                    object keyValue = classType.GetField(keyName).GetValue(classObj);
                    mInfo.Invoke(dicObject, new object[]
                    {
                        keyValue, classObj
                    });
                }

                //把读取之后的表记录下来
                tableDic.Add(typeof(T).Name, containerObj);
            }
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <typeparam name="T">此为Container名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            string tableName = typeof(T).Name;
            if(tableDic.ContainsKey(tableName))
            {
                return tableDic[tableName] as T;
            }
            return null;
        }
    }

}