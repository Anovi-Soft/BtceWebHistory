﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BtceLog
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="BtceTradeHistory")]
	public partial class DataTickersDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertTicker(Ticker instance);
    partial void UpdateTicker(Ticker instance);
    partial void DeleteTicker(Ticker instance);
    #endregion
		
		public DataTickersDataContext() : 
				base(global::BtceLog.Properties.Settings.Default.BtceTradeHistoryConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DataTickersDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataTickersDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataTickersDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataTickersDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Ticker> Tickers
		{
			get
			{
				return this.GetTable<Ticker>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Tickers")]
	public partial class Ticker : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _TickerType;
		
		private System.DateTime _WritingDate;
		
		private double _LastPrice;
		
		private double _BuyPrice;
		
		private double _SellPrice;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnTickerTypeChanging(string value);
    partial void OnTickerTypeChanged();
    partial void OnWritingDateChanging(System.DateTime value);
    partial void OnWritingDateChanged();
    partial void OnLastPriceChanging(double value);
    partial void OnLastPriceChanged();
    partial void OnBuyPriceChanging(double value);
    partial void OnBuyPriceChanged();
    partial void OnSellPriceChanging(double value);
    partial void OnSellPriceChanged();
    #endregion
		
		public Ticker()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TickerType", DbType="NVarChar(7) NOT NULL", CanBeNull=false)]
		public string TickerType
		{
			get
			{
				return this._TickerType;
			}
			set
			{
				if ((this._TickerType != value))
				{
					this.OnTickerTypeChanging(value);
					this.SendPropertyChanging();
					this._TickerType = value;
					this.SendPropertyChanged("TickerType");
					this.OnTickerTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WritingDate", DbType="DateTime NOT NULL")]
		public System.DateTime WritingDate
		{
			get
			{
				return this._WritingDate;
			}
			set
			{
				if ((this._WritingDate != value))
				{
					this.OnWritingDateChanging(value);
					this.SendPropertyChanging();
					this._WritingDate = value;
					this.SendPropertyChanged("WritingDate");
					this.OnWritingDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastPrice", DbType="Float NOT NULL")]
		public double LastPrice
		{
			get
			{
				return this._LastPrice;
			}
			set
			{
				if ((this._LastPrice != value))
				{
					this.OnLastPriceChanging(value);
					this.SendPropertyChanging();
					this._LastPrice = value;
					this.SendPropertyChanged("LastPrice");
					this.OnLastPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BuyPrice", DbType="Float NOT NULL")]
		public double BuyPrice
		{
			get
			{
				return this._BuyPrice;
			}
			set
			{
				if ((this._BuyPrice != value))
				{
					this.OnBuyPriceChanging(value);
					this.SendPropertyChanging();
					this._BuyPrice = value;
					this.SendPropertyChanged("BuyPrice");
					this.OnBuyPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SellPrice", DbType="Float NOT NULL")]
		public double SellPrice
		{
			get
			{
				return this._SellPrice;
			}
			set
			{
				if ((this._SellPrice != value))
				{
					this.OnSellPriceChanging(value);
					this.SendPropertyChanging();
					this._SellPrice = value;
					this.SendPropertyChanged("SellPrice");
					this.OnSellPriceChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
