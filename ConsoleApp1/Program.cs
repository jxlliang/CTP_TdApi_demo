using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using HotDancer.Common;
using TdCTP;
using MdCTP;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var mdapi = MdCTPApiClr.GetInstance();
            mdapi.CreateCenter();
            var path_md = $"{Path.GetFullPath(".")}\\mdApiConPath\\";
            var md_login = new LoginApiInforDataAny<EnumTradeApiTypeAny>
            {
                BrokerID = "",
                UserID = "",
                ApiType = EnumTradeApiTypeAny.CTP期货,
                IPAddress = "3",//41213",
                PassWrod = "",
            };
            unsafe{ mdapi.CreateMdApi_(PIntPtrData.GetIntPtrString(path_md)); }
            unsafe { mdapi.RegisterFront_(PIntPtrData.GetIntPtrString(md_login.IPAddress)); } ///注册行情接口
            mdapi.Init_();   ///初始化行情接口
            //mdapi.logi(td_login);
            unsafe { var date = new String(mdapi.GetTradingDay_()); }
            var tdapi = TdApiClr.GetInstance();
            var path = $"{ Path.GetFullPath(".")}\\tdApiConPath\\";
            //unsafe
            //{
            //    var p = PIntPtrData.GetIntPtrString(path);
            //    tdapi.CreateCenter();
            //    tdapi.CreateTdApi_(p);
            //}
            //MdapiClr mdapi_ = new MdapiClr();
            //MdApiExsample mdapi = new MdApiExsample();
            var login = new LoginApiInforDataAny<EnumTradeApiTypeAny>
            {
                BrokerID="1010",
                UserID="100375",
                ApiType=EnumTradeApiTypeAny.CTP期货,
                IPAddress= "tcp://125.71.232.79:41205",//41213",
                PassWrod="jxl147258",
            };
            ExsampleTdCTPApi tdCTPApi = new ExsampleTdCTPApi();
            tdCTPApi.Login(login);
            //unsafe { tdapi.RegisterFront_(PIntPtrData.GetIntPtrString(login.IPAddress)); } ///注册行情接口
            //tdapi.Init_();   ///初始化行情接口
            //tdapi.ReqUserLogin_()
            //mdapi.Login(login);
            //var date = mdapi.GetTradingDay();

            Application.Run();
        }
    }
}
