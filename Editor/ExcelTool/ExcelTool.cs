using Excel;
using Codice.Client.BaseCommands.WkStatus.Printers;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Text;

public class ExcelTool
{
    //Ҫ��ȡ��Excel�ļ��Ĵ��λ��
    public static string EXCEL_PATH = Application.dataPath + "/Resources/Excel/";
    //���ɵ����ݽṹ��ű����λ��
    public static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/ExcelData/DataClass/";
    //��������λ��
    public static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/DataContainer/";

    private static int BEGIN_INDEX = 4;


    /// <summary>
    /// ��ȡEXCEL_PATH�µ�����excel��������
    /// </summary>
    [MenuItem("Excel Tool/Create Excel Data")]
    private static void CreateExcelData()
    {
        //��ȡ��Ŀ¼�������ļ�����Ϣ
        DirectoryInfo dirInfo = Directory.CreateDirectory(EXCEL_PATH);
        FileInfo[] files = dirInfo.GetFiles();

        DataTableCollection tableCollection;

        for (int i = 0; i < files.Length; i++)
        {
            //ֻ����xlsx�ļ�
            //��֪��Ϊɶ��ȡ����xls
            if (files[i].Extension != ".xlsx" && files[i].Extension != ".xls")
                continue;

            //��ȡExcel
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = reader.AsDataSet().Tables;
                fs.Close();
            }

            //Showһ�µ�ǰexcel������table
            for(int j = 0; j < tableCollection.Count; j++)
            {
                CreateExcelDataClass(tableCollection[j]);
                CreateExcelDataContainer(tableCollection[j]);
                CreateExcelDataBinary(tableCollection[j]);
            }
        }
    }

    /// <summary>
    /// ����DataTable��DATA_CLASS_PATH������class
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataClass(DataTable table)
    {
        if(!Directory.Exists(DATA_CLASS_PATH))
        {
            Directory.CreateDirectory(DATA_CLASS_PATH);
        }

        DataRow nameRow = table.Rows[0];
        DataRow typeRow = table.Rows[1];

        //�༭�ű�������
        string src = "public class" + " " + table.TableName + "\n{\n";

        for(int i = 0; i < table.Columns.Count; i++)
        {
            src += "    public " + typeRow[i].ToString() + " " + nameRow[i].ToString() + ";\n";
        }

        src += "}";

        //��Stringд��ű�
        File.WriteAllText(DATA_CLASS_PATH + table.TableName + ".cs", src);
    }

    /// <summary>
    /// ����DataTable��DATA_CONTAINER_PATH�����ɴ洢���ݵ�Dic
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataContainer(DataTable table)
    {
        int keyIndex = GetKeyIndex(table);
        DataRow typeRow = table.Rows[1];

        if(!Directory.Exists(DATA_CONTAINER_PATH))
        {
            Directory.CreateDirectory(DATA_CONTAINER_PATH);
        }

        string src = "using System.Collections.Generic;\n";
        src += "public class" + " " + table.TableName + "Container" + "\n{\n";

        src += "    public Dictionary<" + typeRow[keyIndex].ToString() + ", " + table.TableName + ">";
        src += " dataDic = new " + "Dictionary<" + typeRow[keyIndex].ToString() + ", " + table.TableName + ">();\n";

        src += "}";

        //��Stringд��ű�
        File.WriteAllText(DATA_CONTAINER_PATH + table.TableName + "Container" + ".cs", src);
    }

    /// <summary>
    /// ����table����2�����ļ�
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataBinary(DataTable table)
    {
        if(!Directory.Exists(ProjectBase.BinaryMgr.DATA_BINARY_PATH))
        {
            Directory.CreateDirectory(ProjectBase.BinaryMgr.DATA_BINARY_PATH);
        }

        using (FileStream fs = new FileStream(ProjectBase.BinaryMgr.DATA_BINARY_PATH + table.TableName + ".zhou", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //�洢Ҫ��ȡ������һ���ж�����
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);

            //��������,��ת����
            string keyname = table.Rows[0][GetKeyIndex(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyname);

            //�洢�������ֵĳ��Ⱥ�����
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes, 0, bytes.Length);

            //��ʼд������
            DataRow row;
            DataRow typeRow = table.Rows[1];
            for(int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for(int j = 0; j < table.Columns.Count; j++)
                {
                    switch(typeRow[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            //stringת���룬��д���䳤�Ⱥ�����
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }
            fs.Close();
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// ��ȡexcel��������key����
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];

        for(int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key")
            {
                return i;
            }
        }
        //��û��key���ѵ�0�е���key
        return 0;
    }
}
