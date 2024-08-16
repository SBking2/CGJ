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

        //string�����ݽṹ������֣�object��Container
        public static Dictionary<string, object> tableDic = new Dictionary<string, object>();


        /// <summary>
        /// ��ȡ������Binary�ļ�������
        /// </summary>
        /// <typeparam name="T">������</typeparam>
        /// <typeparam name="K">������</typeparam>
        public void LoadTable<T, K>()
        {
            using (FileStream fs = File.Open(DATA_BINARY_PATH + typeof(K).Name + ".zhou", FileMode.Open, FileAccess.Read))
            {
                //��ȡ�ļ������ݵ�data��
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                //��ȡָ��
                int index = 0;

                //��ȡrow������
                int rowCount = BitConverter.ToInt32(data, index);
                index += 4;

                //��ȡ�������ֳ��Ⱥ�����
                int keyNameLength = BitConverter.ToInt32(data, index);
                index += 4;
                string keyName = Encoding.UTF8.GetString(data, index, keyNameLength);
                index += keyNameLength;

                //�������������
                Type containerType = typeof(T);
                object containerObj = Activator.CreateInstance(containerType);
                
                Type classType = typeof(K);

                //��ȡ���ݽṹ������ �ֶε���Ϣ
                FieldInfo[] infos = classType.GetFields();

                //��ʼ��ȡ����ÿһ�е���Ϣ
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

                    //��ȡ�� һ������ �����ݼ��뵽������
                    object dicObject = containerType.GetField("dataDic").GetValue(containerObj);

                    MethodInfo mInfo = dicObject.GetType().GetMethod("Add");

                    object keyValue = classType.GetField(keyName).GetValue(classObj);
                    mInfo.Invoke(dicObject, new object[]
                    {
                        keyValue, classObj
                    });
                }

                //�Ѷ�ȡ֮��ı��¼����
                tableDic.Add(typeof(T).Name, containerObj);
            }
        }

        /// <summary>
        /// ��ȡ��
        /// </summary>
        /// <typeparam name="T">��ΪContainer��</typeparam>
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