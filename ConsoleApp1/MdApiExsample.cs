using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MdCTP;
using HotDancer.Common;
using HotDancer.Common.BaseMdApi;
using System.IO;
namespace ConsoleApp1
{
   public class MdApiExsample:IBaseMdApi
    {
        /// <summary>
        /// 返回登录信息
        /// </summary>
        public LoginApiInforDataAny<EnumQuoteApiTypeAny> LoginInfo { get; private set; }

        /// <summary>
        /// 返回ApiType
        /// </summary>
        public EnumQuoteApiTypeAny ApiType => LoginInfo.ApiType;

        /// <summary>
        /// 返回所有订阅的code
        /// </summary>
        public List<string> AllSubMarketCodes { get; } = new List<string>();

        public event Action<IBaseMdApi, OnQuoteMarketDataAny> OnQuoteData;
        public event Action<string> OnLog;
        MdapiClr MdapiClr_;
        int reqID = 0;

        public MdApiExsample()
        {
            MdapiClr_ = MdapiClr.GetInstance();
            var path = $"{ Path.GetFullPath(".")}\\mdApiConPath\\";
            ///设置回调函数的绑定
            OnFrontConnectedDelegate onConnectDelegate = OnFrontConnected_;
            OnFrontDisconnectedDelegate onDisConnectDelegate = OnFrontDisconnected_;
            OnRspUserLoginDelegate onLoginDelegate = OnRspUserLogin_;
            OnRspUserLogoutDelegate onLogoutDelegate = OnRspUserLogout_;
            OnRspErrorDelegate onErrorDelegate = OnRspError_;
            OnRtnDepthMarketDataDelegate onMarketDataDelegate = OnRtnDepthMarketData_;

            MdapiClr_.SetOnFrontConnectedDelegate(onConnectDelegate);
            MdapiClr_.SetOnFrontDisConnectedDelegate(onDisConnectDelegate);
            MdapiClr_.SetOnRspUserLoginDelegate(onLoginDelegate);
            MdapiClr_.SetOnRspUserLogoutDelegate(onLogoutDelegate);
            MdapiClr_.SetOnRspErrorDelegate(onErrorDelegate);
            MdapiClr_.SetOnRtnDepthMarketDataDelegate(onMarketDataDelegate);
            unsafe
            {
                var p = PIntPtrData.GetIntPtrString(path);
                MdapiClr_.CreateCenter();
                MdapiClr_.CreateMdApi_(p);
            }
        }
        /// <summary>
        /// 返回当前日期
        /// </summary>
        /// <returns></returns>
        public string GetTradingDay()
        {
            unsafe { return new String(MdapiClr_.GetTradingDay_()); }
        }
        MdapiClr.ReqUserLogin userLogin;
        public void Login(LoginApiInforDataAny loginData)
        {
            LoginInfo = loginData;
            userLogin = new MdapiClr.ReqUserLogin
            {
                BrokerID = loginData.BrokerID,
                UserID = loginData.UserID,
                Password = loginData.PassWrod
            };
            unsafe { MdapiClr_.RegisterFront_(PIntPtrData.GetIntPtrString(loginData.IPAddress)); } ///注册行情接口
            MdapiClr_.Init_();   ///初始化行情接口
        }

        /// <summary>
        /// 行情接口连接回报
        /// </summary>
        public void OnFrontConnected_()
        {
            var log = "行情接口连接成功";
            reqID += 1;
            MdapiClr_.ReqUserLogin_(userLogin, reqID);
            OnLog?.Invoke(log);
        }
        /// <summary>
        /// 行情接口断开回报
        /// </summary>
        /// <param name="n"></param>
        public void OnFrontDisconnected_(int n)
        {
            var log = "行情接口断开连接";
            OnLog?.Invoke(log);
        }

        /// <summary>
        /// 订阅行情
        /// </summary>
        /// <param name="code"></param>
        public void SubMarketData(string code)
        {
            AllSubMarketCodes.Add(code);
            unsafe { MdapiClr_.SubscribeMarketData_(PIntPtrData.GetIntPtrString(code)); }
        }
        /// <summary>
        /// 退订行情
        /// </summary>
        /// <param name="code"></param>
        public void UnSubMarketData(string code)
        {
            unsafe { MdapiClr_.UnSubscribeMarketData_(PIntPtrData.GetIntPtrString(code)); }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="error"></param>
        /// <param name="n"></param>
        /// <param name="b"></param>
        public void OnRspError_<T>(T error, int n, bool b)
        {
            object obj = error;
            ErrorInfo error_d = (ErrorInfo)obj;
            var log = $"CTP行情账号错误回报，错误代码：{error_d.ErrorID},错误信息：{error_d.ErrorMsg}";
            OnLog?.Invoke(log);
        }
        /// <summary>
        /// 行情账号登录回报
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="onUserLogin"></param>
        /// <param name="error"></param>
        /// <param name="n"></param>
        /// <param name="b"></param>
        public void OnRspUserLogin_<T1, T2>(T1 onUserLogin, T2 error, int n, bool b)
        {
            object obj = error;
            ErrorInfo error_d = (ErrorInfo)obj;
            if (error_d.ErrorID == 0)
                OnLog?.Invoke($"CTP行情账号登录成功");
            else
                OnLog?.Invoke($"CTP行情账号登录失败，错误信息:{error_d.ErrorMsg}");
        }
        /// <summary>
        /// 行情账号登出回报
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="onUserLogOut"></param>
        /// <param name="error"></param>
        /// <param name="n"></param>
        /// <param name="b"></param>
        public void OnRspUserLogout_<T1, T2>(T1 onUserLogOut, T2 error, int n, bool b)
        {
            object obj = error;
            ErrorInfo error_d = (ErrorInfo)obj;
            if (error_d.ErrorID == 0)
                OnLog?.Invoke($"CTP行情账号登出成功");
            else
                OnLog?.Invoke($"CTP行情账号登出失败，错误信息:{error_d.ErrorMsg}");
        }
        /// <summary>
        /// 行情信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onDepthMarketData"></param>
        public void OnRtnDepthMarketData_<T>(T onDepthMarketData)
        {
            object obj = onDepthMarketData;
            DepthMarketData marketData = (DepthMarketData)obj;
            OnQuoteMarketDataAny marketDataAny = new OnQuoteMarketDataAny
            {
                AskPrice1 = marketData.AskPrice1,
                AskPrice2 = marketData.AskPrice2,
                AskPrice3 = marketData.AskPrice3,
                AskPrice4 = marketData.AskPrice4,
                AskPrice5 = marketData.AskPrice5,
                AskVolume1 = marketData.AskVolume1,
                AskVolume2 = marketData.AskVolume2,
                AskVolume3 = marketData.AskVolume3,
                AskVolume4 = marketData.AskVolume4,
                AskVolume5 = marketData.AskVolume5,
                BidPrice1 = marketData.BidPrice1,
                BidPrice2 = marketData.BidPrice2,
                BidPrice3 = marketData.BidPrice3,
                BidPrice4 = marketData.BidPrice4,
                BidPrice5 = marketData.BidPrice5,
                BidVolume1 = marketData.BidVolume1,
                BidVolume2 = marketData.BidVolume2,
                BidVolume3 = marketData.BidVolume3,
                BidVolume4 = marketData.BidVolume4,
                BidVolume5 = marketData.BidVolume5,
                Code = marketData.InstrumentID,
                ExchangeID = (EnumExchangeIDTypeAny)Enum.Parse(typeof(EnumExchangeIDTypeAny), marketData.ExchangeID),
                ExchangeInstID = marketData.ExchangeInstID,
                HighestPrice = marketData.HighestPrice,
                LastPrice = marketData.LastPrice,
                LowerLimitPrice = marketData.LowerLimitPrice,
                LowestPrice = marketData.LowestPrice,
                OpenInterest = marketData.OpenInterest,
                OpenPrice = marketData.OpenPrice,
                PreClosePrice = marketData.PreClosePrice,
                PreOpenInterest = marketData.PreOpenInterest,
                PreSettlementPrice = marketData.PreSettlementPrice,
                SettlementPrice = marketData.SettlementPrice,
                TradingDay = marketData.TradingDay,
                Turnover = marketData.Turnover,
                UpdateMillisec = marketData.UpdateMillisec,
                UpdateTime = marketData.UpdateTime,
                UpperLimitPrice = marketData.UpperLimitPrice,
                Volume = marketData.Volume
            };

            if (marketData != null)
                OnQuoteData?.Invoke(this, marketDataAny);
        }
    }
}
