using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProjectBase
{
    /// <summary>
    /// ����һ��key����keyName_��������_�ֶ�����_�ֶ���
    /// ����List ���� keyName_��������_List_�ֶ��� ��ʾ list.Count  ����index��ʾԪ��
    /// ����Dictionary keyName_��������_Dictionary_�ֶ��� ��ʾ dictionary.Count ��� _key_index ���� _value_index
    /// </summary>
    public class PlayerPrefsMgr : BaseManager<PlayerPrefsMgr>
    {
        /// <summary>
        /// �洢����
        /// </summary>
        /// <param name="className"></param>
        /// <param name="data"></param>
        public void SaveData(string keyName, object data)
        {
            Type dataType = data.GetType();
            FieldInfo[] fieldInfos = dataType.GetFields();

            for(int i = 0; i < fieldInfos.Length; i++)
            {
                //Save �ֶ�
                SaveValue(keyName + "_" + dataType.Name + "_" + fieldInfos[i].FieldType.Name + "_" + fieldInfos[i].Name
                    , fieldInfos[i].GetValue(data));
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Ϊ�ֶε�����ֵ
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        private void SaveValue(string keyName, object value)
        {
            Debug.Log(value);
            Type fieldType = value.GetType();

            if(fieldType == typeof(int))
            {
                PlayerPrefs.SetInt(keyName, (int)value);
            }
            else if(fieldType == typeof(float))
            {
                PlayerPrefs.SetFloat(keyName, (float)value);
            }
            else if(fieldType == typeof(string))
            {
                PlayerPrefs.SetString(keyName, value.ToString());
            }
            else if(fieldType == typeof(bool))
            {
                PlayerPrefs.SetInt(keyName, (bool)value ? 1 : 0);
            }
            //��IList���Է���fieldType�Ŀռ䣬˵��fieldType��IList�����࣬˵����List
            //List��Key����List�ĳ��ȣ��������index��ʾԪ��
            else if(typeof(IList).IsAssignableFrom(fieldType))
            {
                IList list = value as IList;
                //�ȼ�¼list����
                PlayerPrefs.SetInt(keyName, list.Count);

                //��Index��ʾ�ڼ���Ԫ��
                int index = 0;
                foreach(object obj in list)
                {
                    SaveValue(keyName + index, obj);
                    index++;
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                IDictionary dic = value as IDictionary;
                //�ȼ�¼list����
                PlayerPrefs.SetInt(keyName, dic.Count);

                //��Index��ʾ�ڼ���Ԫ��
                int index = 0;
                foreach (object key in dic.Keys)
                {
                    SaveValue(keyName + "_key_" + index, key);
                    SaveValue(keyName + "_value_" + index, dic[key]);
                    index++;
                }
            }
            //�����Զ�����ֱ�ӵݹ鼴��
            else
            {
                SaveData(keyName, value);
            }

        }

        /// <summary>
        /// �����Լ��趨�ļ�����ȡ����
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadData(string keyName, Type type)
        {
            object data = Activator.CreateInstance(type);

            //��ȡ�ֶ���Ϣ
            FieldInfo[] fieldInfos = type.GetFields();

            for(int i = 0; i < fieldInfos.Length;i++)
            {
                Type fieldType = fieldInfos[i].FieldType;
                string fieldName = fieldInfos[i].Name;

                string key = keyName + "_" + type.Name + "_" + fieldType.Name + "_" + fieldName;

                //��ȡ�ֶ�ֵ����ֵ��data
                fieldInfos[i].SetValue(data, LoadValue(key, fieldType));

            }

            return data;
        }

        /// <summary>
        /// ��ȡ�����ֶ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadValue(string key, Type type)
        {
            if(type == typeof(int))
            {
                return PlayerPrefs.GetInt(key, 0);
            }else if(type == typeof(float))
            {
                return PlayerPrefs.GetFloat(key, 0);
            }else if (type == typeof(string))
            {
                return PlayerPrefs.GetString(key, "");
            }else if(type == typeof(bool))
            {
                return PlayerPrefs.GetInt(key, 0) == 1 ? true : false;
            }
            else if(typeof(IList).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key, 0);

                IList list = Activator.CreateInstance(type) as IList;
                for(int i = 0; i < count; i++)
                {
                    list.Add(LoadValue(key + i, type.GetGenericArguments()[0]));
                }
                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key, 0);

                IDictionary dic = Activator.CreateInstance(type) as IDictionary;
                Type[] types = type.GetGenericArguments();
                for (int i = 0; i < count; i++)
                {
                    dic.Add(
                        LoadValue(key + "_key_" + i, types[0])
                        , LoadValue(key + "_value_" + i, types[1])
                        );
                }
                return dic;
            }else
            {
                return LoadData(key, type);
            }
        }
    }
}
