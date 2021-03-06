﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace MidaxLib
{
    public class Trade
    {
        string _epic;
        SIGNAL_CODE _direction = SIGNAL_CODE.UNKNOWN;
        int _size = 0;
        string _reference;
        string _id;
        DateTime _tradingTime = DateTime.MinValue;
        DateTime _confirmationTime = DateTime.MinValue;
        decimal _price;
        int _placeHolder = 0;
        TradeCancelled _tradeCancelled;
        
        public string Epic { get { return _epic; } }
        public SIGNAL_CODE Direction { get { return _direction; } set { _direction = value; } }
        public int Size { get { return _size; } set { _size = value; } }
        public string Id { get { return _id; } set { _id = value; } }
        public string Reference { get { return _reference; } set { _reference = value; } }
        public DateTime TradingTime { get { return _tradingTime; } }
        public DateTime ConfirmationTime { get { return _confirmationTime; } set { _confirmationTime = value; } }
        public decimal Price { get { return _price; } set { _price = value; } }
        public int PlaceHolder { get { return _placeHolder; } set { _placeHolder = value; } }

        public delegate void TradeCancelled(DateTime cancelTime, decimal stockValue, bool openPosition);

        public Trade(DateTime tradingTime, string epic, SIGNAL_CODE direction, int size, decimal price, int placeHolder = 0, TradeCancelled onTradeCancelled = null)
        {
            _tradingTime = tradingTime;
            _epic = epic;
            _direction = direction;
            _size = size;
            _price = price;
            _placeHolder = placeHolder;
            _tradeCancelled = onTradeCancelled;
        }

        public Trade(Trade cpy, bool opposite = false, DateTime? trading_time = null)
        {
            this._epic = cpy._epic;
            if (opposite)
            {
                this._direction = (cpy._direction == SIGNAL_CODE.BUY ? SIGNAL_CODE.SELL : SIGNAL_CODE.BUY);
                this._tradingTime = trading_time.Value;
            }
            else
            {
                this._direction = cpy._direction;
                this._tradingTime = cpy._tradingTime;
                this._confirmationTime = cpy._confirmationTime;
            }
            this._size = cpy._size;
            this._reference = cpy._reference;
            this._id = cpy._id;
            this._price = cpy._price;
            this._placeHolder = cpy._placeHolder;
            this._tradeCancelled = cpy._tradeCancelled;
        }

        public Trade(Row row)
        {
            _id = (string)row[0];
            _confirmationTime = new DateTime(((DateTimeOffset)row[1]).Ticks, DateTimeKind.Local);
            _direction = (SIGNAL_CODE)(int)row[2];
            _epic = (string)row[3];
            _price = (decimal)row[4];
            _size = (int)row[5];
            _reference = (string)row[6];
            _tradingTime = new DateTime(((DateTimeOffset)row[7]).Ticks, DateTimeKind.Local);
        }

        public void Publish()
        {
            PublisherConnection.Instance.Insert(this);
        }

        public void OnRejected(DateTime cancelTime, decimal stockValue, bool openPosition)
        {
            if (_tradeCancelled != null)
                _tradeCancelled(cancelTime, stockValue, openPosition);
        }
    }
}
