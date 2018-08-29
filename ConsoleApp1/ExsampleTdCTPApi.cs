using System;
using System.Collections.Generic;
using System.IO;
using HotDancer.Common;
using TdCTP;
using HotDancer.Common.BaseTdApi;
namespace ConsoleApp1
{
    public class ExsampleTdCTPApi
    {
        public EnumTradeApiTypeAny ApiType => loginInfoField.ApiType;
        public LoginApiInforDataAny<EnumTradeApiTypeAny> LoginInfo => loginInfoField;
        public event Action<IBaseTdApi> TradeApiConnected;
        public event Action<IBaseTdApi, int> TradeApiDisConnected;
        public event Action<IBaseTdApi, OrderReturnInforDataAny> OnRtnOrder;
        public event Action<IBaseTdApi, TradeReturnInforDataAny> OnRtnTrade;
        public event Action<IBaseTdApi, OrderReturnInforDataAny, ErrorInfoAny, int, bool> OnRspOrderAction;
        public event Action<IBaseTdApi, OrderInsertInfoDataAny, ErrorInfoAny, int, bool> OnRspOrderInsert;
        public event Action<IBaseTdApi, OnRspQryInvestorPositionAny, ErrorInfoAny, int, bool> OnRspQryInvestorPosition;
        public event Action<IBaseTdApi, string, ErrorInfoAny, int, bool> OnRspQrySettlementInfo;
        public event Action<IBaseTdApi, OnTradingAccountFieldAny, ErrorInfoAny, int, bool> OnRspQryTradingAccount;
        public event Action<IBaseTdApi, OnRspUserLoginAny, ErrorInfoAny, int, bool> OnRspUserLogin;
        public event Action<IBaseTdApi, OnRspUserLogOutAny, ErrorInfoAny, int, bool> OnRspUserLogout;
        public event Action<IBaseTdApi, OrderReturnInforDataAny, ErrorInfoAny, int, bool> OnRspQryOrder;
        public event Action<IBaseTdApi, TradeReturnInforDataAny, ErrorInfoAny, int, bool> OnRspQryTrade;
        public event Action<string> OnLog;

        TdApiClr tdApi;
        int reqID=0;
        public ExsampleTdCTPApi()
        {
            tdApi = TdApiClr.GetInstance();
            var path = $"{ Path.GetFullPath(".")}\\tdApiConPath\\";
            tdApi.CreateCenter();
            unsafe{ tdApi.CreateTdApi_(PIntPtrData.GetIntPtrString(path)); }

            OnFrontConnectedDelegate onFrontConnectDelegate = OnFrontConnected_;
            OnFrontDisconnectedDelegate onFrontDisconnectedDelegate = OnFrontDisconnected_;
            OnAuthenticateDelegate onAuthenticateDelegate = OnRspAuthenticate_;
            OnRspUserLoginDelegate onRspUserLoginDelegate = OnRspUserLogin_;
            OnRspUserLogoutDelegate onRspUserLogoutDelegate = OnRspUserLogout_;
            OnRspErrorDelegate onRspErrorDelegate = OnRspError_;
            //OnRspOrderInsertDelegate(OrderInsert^ orderInsert, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspOrderActionDelegate(OrderAction^ orderAction, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspSettlementInfoConfirmDelegate(RspSettlementInfoConfirm^ settelment, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQuoteInsertDelegate(RspInputQuoteField^ inputQuote, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQuoteActionDelegate(RspQuoteActionField^ action, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQryInvestorPositionDelegate(RspInvestorPositionField^ investorPosition, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQryTradingAccountDelegate(RspTradingAccountField^ tradingAccount, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQryInvestorDelegate(RspInvestorField^ investor, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQryInstrumentDelegate(RspInstrumentField^ code, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQrySettlementInfoDelegate(RspSettlementInfoField^ settlementInfo, ErrorInfo^ pRspInfo, int pResquesID, bool bIsLast);
            //OnRspQryInvestorPositionDetailDelegate(RspInvestorPositionDelailField^ investorPositionDelail, ErrorInfo^ pRspInfo, int pRequestID, bool bIsLast);
            //OnRspQrySettlementInfoConfirmDelegate(RspSettlementInfoConfirm^ settelment, ErrorInfo^ pRspInfo, int nRequestID, bool bIsLast);
            //OnRspQryInvestorPositionCombineDetailDelegate(RspInvestorPositionCombineDetailField^ investorPositionCombineDetail, ErrorInfo^ pRequestID, int nRequestID, bool bIsLast);
            //OnRtnOrderDelegate(RtnOrderField^ rtnOrder);
            //OnRtnTradeDelegate(RtnTradeField^ rtnTrade);
            tdApi.SetOnFrontConnectedDelegate(onFrontConnectDelegate);
            tdApi.SetOnFrontDisconnectedDelegate(onFrontDisconnectedDelegate);
            tdApi.SetOnRspErrorDelegate(onRspErrorDelegate);
            tdApi.SetAuthenDelegate(onAuthenticateDelegate);
            tdApi.SetOnRspUserLoginDelegate(onRspUserLoginDelegate);
            tdApi.SetOnRspUserLogoutDelegate(onRspUserLogoutDelegate);
        }
        
        public string GetTradingDay()
        {
            unsafe { return new String(tdApi.GetTradingDay_()); }
        }

        public void Init() => tdApi.Init_();

        public int Join() => tdApi.Join_();

        LoginApiInforDataAny<EnumTradeApiTypeAny> loginInfoField;
        TdApiClr.ReqUserLoginField reqLogin;
        public void Login(LoginApiInforDataAny<EnumTradeApiTypeAny> loginData)
        {
            loginInfoField = loginData;
            reqLogin = new TdApiClr.ReqUserLoginField
            {
                BrokerID = loginData.BrokerID,
                ClientIPAddress = loginData.IPAddress,
                Password = loginData.PassWrod,
                UserID = loginData.UserID
            };
            tdApi.SubscribePublicTopic_();   //订阅公有流
            tdApi.SubscribePrivateTopic_(); //订阅私有流
            unsafe{ tdApi.RegisterFront_(PIntPtrData.GetIntPtrString(loginData.IPAddress)); }
            tdApi.Init_();
        }

        public void Logout(LoginApiInforDataAny<EnumTradeApiTypeAny> logOutData)
        {
            throw new NotImplementedException();
        }

        public string OrderInsert(string InstrumentID, EnumDirectionTypeAny Direction, EnumOffsetFlagTypeAny OffsetFlag, double LimitPrice, int VolumeTotalOriginal, string OrderPriceType = "2")
        {
            throw new NotImplementedException();
        }

        public int QrySettlementInfo()
        {
            TdApiClr.ReqSettlementInfoField reqSettlement = new TdApiClr.ReqSettlementInfoField()
            {
                BrokerID= loginInfoField.BrokerID,
                InvestorID=loginInfoField.UserID
            };
            reqID += 1;
            return tdApi.ReqQrySettlementInfo_(reqSettlement, reqID);
        }

        public int ReqQryInstrument()
        {
            TdApiClr.ReqInstrumentField reqInstrument = new TdApiClr.ReqInstrumentField();
            reqID += 1;
            return tdApi.ReqQryInstrument_(reqInstrument, reqID);
        }
        /// <summary>
        /// 查询投资者
        /// </summary>
        /// <returns></returns>
        public int ReqQryInvestor()
        {
            TdApiClr.ReqInvestorField reqInvestor = new TdApiClr.ReqInvestorField()
            {
                BrokerID= loginInfoField.BrokerID,
                InvestorID= loginInfoField.UserID
            };
            reqID += 1;
            return tdApi.ReqQryInvestor_(reqInvestor, reqID);
        }

        public int ReqQryTradingAccount()
        {
            reqID += 1;
            TdApiClr.ReqTradingAccountField account = new TdApiClr.ReqTradingAccountField()
            {
                BrokerID = loginInfoField.BrokerID,
                InvestorID = loginInfoField.UserID
            };
           return tdApi.ReqQryTradingAccount_(account, reqID);
        }

       
        //////////////////////////回调函数/////////////////////////↓↓
       
        public void OnFrontConnected_()
        {
            reqID += 1;
            tdApi.ReqUserLogin_(reqLogin, reqID);
        }

        public void OnFrontDisconnected_(int nReason)
        {
            reqID += 1;
            //tdApi.ReqUserLogout_()
        }

        public void OnRspAuthenticate_<T1, T2>(T1 pRspAuthenticateField, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspUserLogin_<T1, T2>(T1 pRspUserLogin, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            ///登录回报
            object obj = pRspInfo;
            var error = (ErrorInfo)obj;
            if(error.ErrorID==0)
                OnLog?.Invoke($"CTP交易账户登录成功:{error.ErrorMsg}");
            else
            {
                OnLog?.Invoke($"CTP交易账户登录失败:{error.ErrorMsg}");
            }
        }

        public void OnRspUserLogout_<T1, T2>(T1 pUserLogout, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            ///登出回报
            object obj = pRspInfo;
            var error = (ErrorInfo)obj;
            if (error.ErrorID == 0)
                OnLog?.Invoke($"CTP交易账户登出成功:{error.ErrorMsg}");
            else
            {
                OnLog?.Invoke($"CTP交易账户登出失败:{error.ErrorMsg}");
            }
        }
        /// <summary>
        /// 委托回报
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pInputOrder">委托信息</param>
        /// <param name="pRspInfo">错误信息</param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        public void OnRspOrderInsert_<T1, T2>(T1 pInputOrder, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            object obj = pRspInfo;
            var error = (ErrorInfo)obj;
            if (error.ErrorID == 0)
                OnLog?.Invoke($"委托成功回报:{error.ErrorMsg}");
            else
            {
                OnLog?.Invoke($"委托失败回报:{error.ErrorMsg}");
            }
        }

        public void OnRspOrderAction_<T1, T2>(T1 pInputOrderAction, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspSettlementInfoConfirm_<T1, T2>(T1 pSettlementInfoConfirm, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQuoteInsert_<T1, T2>(T1 pInputQuote, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQuoteAction_<T1, T2>(T1 pInputQuoteAction, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryInvestorPosition_<T1, T2>(T1 pInvestorPosition, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryTradingAccount_<T1, T2>(T1 pTradingAccount, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryInvestor_<T>(T pInvestor, T pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryInstrument_<T1, T2>(T1 pInstrument, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQrySettlementInfo_<T1, T2>(T1 pSettlementInfo, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryInvestorPositionDetail_<T1, T2>(T1 pInvestorPositionDetail, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQrySettlementInfoConfirm_<T1, T2>(T1 pSettlementInfoConfirm, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspQryInvestorPositionCombineDetail_<T1, T2>(T1 pInvestorPositionCombineDetail, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            throw new NotImplementedException();
        }

        public void OnRspError_<T>(T pRspInfo, int nRequestID, bool bIsLast)
        {
            object obj = pRspInfo;
            var error = (ErrorInfo)obj;
            var log = $"CTP交易账号错误回报，错误代码：{error.ErrorID},错误信息：{error.ErrorMsg}";
            OnLog?.Invoke(log);

        }

        public void OnRtnOrder_<T>(T pOrder)
        {
            object obj = pOrder;
            var rtnOrderField = (RtnOrderField)obj;
            OrderReturnInforDataAny onOrderInfo = new OrderReturnInforDataAny
            {
                OrderCancelTime=rtnOrderField.CancelTime,
                OrderInsertTime=rtnOrderField.InsertTime,
                OrderUpdateTime=rtnOrderField.UpdateTime,
                BrokerID=rtnOrderField.BrokerID,
                Code=rtnOrderField.InstrumentID,
                OrderDirection=ChangeDataTypeAny.GetDirectionType(rtnOrderField.Direction),
                OrderExchangeID=ChangeDataTypeAny.GetExchangIDType(rtnOrderField.ExchangeID),
                FrontID=rtnOrderField.FrontID,
                OrderHedgeFlag=ChangeDataTypeAny.GetHedgeFlagType(rtnOrderField.CombHedgeFlag),
                InvestorID=rtnOrderField.InvestorID,
                OrderPrice=rtnOrderField.LimitPrice,
                OrderOffsetFlag=ChangeDataTypeAny.GetOffsetFlagType(rtnOrderField.CombOffsetFlag),
                OrderRef=rtnOrderField.OrderRef,
                OrderStatus=ChangeDataTypeAny.GetOrderStatusType(rtnOrderField.OrderStatus),
                OrderSysID=rtnOrderField.OrderSysID,
                SessionID=rtnOrderField.SessionID,
                OrderStatusMsg=rtnOrderField.StatusMsg,
                TradingDay=rtnOrderField.TradingDay,
                UserID=rtnOrderField.UserID,
                VolumeTotal=rtnOrderField.VolumeTotal,
                VolumeTotalOriginal=rtnOrderField.VolumeTotalOriginal,
                VolumeTraded=rtnOrderField.VolumeTraded
            };
            //OnRtnOrder?.Invoke(this, onOrderInfo);
        }

        public void OnRtnTrade_<T>(T pTrade)
        {
            object obj = pTrade;
            var rtnTradeField = (RtnTradeField)obj;
            TradeReturnInforDataAny tradeReturnInfor = new TradeReturnInforDataAny
            {
                BrokerID = rtnTradeField.BrokerID,
                Code = rtnTradeField.InstrumentID,
                InvestorID = rtnTradeField.InvestorID,
                OrderRef = rtnTradeField.OrderRef,
                OrderSysID = rtnTradeField.OrderSysID,
                TradeOffsetFlag = ChangeDataTypeAny.GetOffsetFlagType(rtnTradeField.OffsetFlag),
                TradeHedgeFlag =ChangeDataTypeAny.GetHedgeFlagType(rtnTradeField.HedgeFlag),
                TradeDate=rtnTradeField.TradeDate,
                TradeTime=rtnTradeField.TradeTime,
                TradeDirection=ChangeDataTypeAny.GetDirectionType(rtnTradeField.Direction),
                TradeExchangeID = ChangeDataTypeAny.GetExchangIDType(rtnTradeField.ExchangeID),
                TradeID=rtnTradeField.TradeID,
                TradePrice=rtnTradeField.Price,
                TradeVolume=rtnTradeField.Volume,
                TradingDay=rtnTradeField.TradingDay,
                UserID=rtnTradeField.UserID,
            };
        }
        /// <summary>
        /// 查询投资者响应
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pInvestor"></param>
        /// <param name="pRspInfo"></param>
        /// <param name="nRequestID"></param>
        /// <param name="bIsLast"></param>
        public void OnRspQryInvestor_<T1, T2>(T1 pInvestor, T2 pRspInfo, int nRequestID, bool bIsLast)
        {
            object obj = pRspInfo;
            var error = (ErrorInfo)obj;
            if (error.ErrorID == 0)
                OnLog?.Invoke($"查询投资者成功:{error.ErrorMsg}");
            else
            {
                OnLog?.Invoke($"查询投资者失败:{error.ErrorMsg}");
            }
        }

        public void Release()
        {
            tdApi.Release_();
        }
    }
}
