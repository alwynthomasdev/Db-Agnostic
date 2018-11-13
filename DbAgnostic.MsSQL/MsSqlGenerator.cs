using System;
using System.Text;

namespace DbAgnostic
{
    internal class MsSqlGenerator<T> : SqlGenerator<T>
    {
        protected override string GenerateSelectStatement()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT ");

            for (int i = 0; i < SelectList.Length; i++)
            {
                sb.AppendFormat("{0} [{1}]{2}\n",
                    TAB,
                    SelectList[i],
                    SelectList.Length == i + 1 ? "" : ",");//if its the last property
            }

            sb.AppendLine("FROM");
            sb.AppendFormat("[{0}]", TableName);

            return sb.ToString();
        }

        protected override string GenerateSelectByIdSatement()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT TOP 1");

            for (int i = 0; i < SelectList.Length; i++)
            {
                sb.AppendFormat("{0} [{1}]{2}\n",
                    TAB,
                    SelectList[i],
                    SelectList.Length == i + 1 ? "" : ",");//if its the last property
            }

            sb.AppendLine("FROM");
            sb.AppendFormat("[{0}]", TableName);
            sb.AppendLine("WHERE");
            sb.AppendFormat("{0} [{1}] = @ID", TAB, KeyColumnName);

            return sb.ToString();
        }

        protected override string GenerateInsertSatement()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("INSERT INTO [{0}]\n", TableName);

            string cols = "";
            string vals = "";
            bool isFirst = true;//used to check if the current property is the first to be inserted
            for (int i = 0; i < InsertList.Length; i++)
            {
                //column list
                cols += string.Format("{0}{2}[{1}]{3}\n",
                    TAB,
                    InsertList[i],
                    isFirst ? "(" : ",",                               //if its the first '(' else ','
                    InsertList.Length == i + 1 ? ")" : "");   //if its the last property

                //value list
                vals += string.Format("{0}{2}@{1}{3}",
                    TAB,
                    InsertList[i],
                    isFirst ? "(" : ",",                               //if its the first '(' else ','
                    InsertList.Length == i + 1 ? ")" : "\n");   //if its the last property

                isFirst = false;//no longer first value after one full interation
            }
            sb.Append(cols);
            sb.AppendLine("VALUES");
            sb.Append(vals);

            //add select query if there is a key column name
            if (!string.IsNullOrWhiteSpace(KeyColumnName))
            {
                sb.AppendLine("\n\nSELECT TOP 1");
                for (int i = 0; i < SelectList.Length; i++)
                {
                    sb.AppendFormat("{0}[{1}]{2}\n",
                        TAB,
                        SelectList[i],
                        SelectList.Length == i + 1 ? "" : ",");//if its the last property
                }
                sb.AppendLine("FROM");
                sb.AppendFormat("{0}[{1}]\n", TAB, TableName);
                sb.AppendLine("WHERE");
                sb.AppendFormat("{0}[{1}] = SCOPE_IDENTITY()", TAB, KeyColumnName);
            }

            return sb.ToString();
        }

        protected override string GenerateUpdateSatement()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE [{0}]\n SET\n", TableName);

            for (int i = 0; i < UpdateList.Length; i++)
            {
                sb.AppendFormat("{2}[{0}] = @{0}{1}\n",
                    UpdateList[i],
                    UpdateList.Length == i + 1 ? "" : ",",
                    TAB);
            }

            sb.AppendFormat("WHERE [{0}] = @{0}", KeyColumnName);

            return sb.ToString();
        }

        protected override string GenerateDeleteSatement()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM [{0}] WHERE [{1}] = @ID", TableName, KeyColumnName);
            return sb.ToString();
        }

        protected override string GenerateSelectByStatement(string propertyName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(SelectSatement);
            sb.AppendLine("\nWHERE");
            sb.AppendFormat("{0}[{1}] = @Value", TAB, propertyName);

            return sb.ToString();
        }

        protected override string GenerateSelectSingleByStatement(string propertyName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT TOP 1");
            for (int i = 0; i < SelectList.Length; i++)
            {
                sb.AppendFormat("{0} [{1}]{2}\n",
                    TAB,
                    SelectList[i],
                    SelectList.Length == i + 1 ? "" : ",");//if its the last property
            }
            sb.AppendLine("FROM");
            sb.AppendFormat("[{0}]", TableName);
            sb.AppendLine("WHERE");
            sb.AppendFormat("{0}[{1}] = @Value", TAB, propertyName);

            return sb.ToString();
        }

        protected override string GenerateUpdateByStatement(string propertyName)
        {
            //TODO: implement this
            throw new NotImplementedException();
        }

        protected override string GenerateDeleteByStatement(string propertyName)
        {
            //TODO: implement this
            throw new NotImplementedException();
        }
    }
}
