//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using SmsSendAndReceive;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
       // public const string FEATURE_NAME = "SmsSendAndReceive";
        public const string FEATURE_NAME = "Antidaephobiax - Be Safe";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Be Safe - Send SMS", ClassType=typeof(SendTextMessage)},
          //  new Scenario() { Title="Sms Send and Receive", ClassType=typeof(RegisterWithBackgroundTask)}
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
