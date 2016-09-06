﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;


    /// <summary>
    /// Should be implemented by containers that support generic object resolution in order to 
    /// provide a common lifetime management policy for all activities
    /// </summary>
    public interface ActivityFactory
    {
        /// <summary>
        /// Create and execute the activity
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Execute<TActivity, TArguments>(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TArguments>> next)
            where TActivity : ExecuteActivity<TArguments>
            where TArguments : class;

        /// <summary>
        /// Create and compensate the activity
        /// </summary>
        /// <param name="compensateContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Compensate<TActivity, TLog>(CompensateContext<TLog> compensateContext, IPipe<CompensateActivityContext<TLog>> next)
            where TActivity : CompensateActivity<TLog>
            where TLog : class;
    }


    public interface ActivityFactory<TArguments, TLog> :
        ExecuteActivityFactory<TArguments>,
        CompensateActivityFactory<TLog>
        where TArguments : class
        where TLog : class
    {
    }
}