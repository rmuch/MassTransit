// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using System.Runtime.Serialization;
	using Magnum.Common.StateMachine;
	using MassTransit.Saga;
	using Messages;

	[Serializable]
	public class RegisterUserStateMachine :
		SagaStateMachine<RegisterUserStateMachine>,
		ISaga,
		InitiatedBy<RegisterUser>,
		Orchestrates<UserValidated>
	{
		static RegisterUserStateMachine()
		{
			Define(() =>
				{
					Initially(
						When(NewUserRegistration, (w, e, m) =>
							{
								w.Username = m.Username;
								w.Password = m.Password;
								w.DisplayName = m.DisplayName;
								w.Email = m.Email;

								// e.Message would include the message
								w.TransitionTo(WaitingForEmailValidation);
							}));

					During(WaitingForEmailValidation,
						When(EmailValidated, (w) =>
							{
								w.Validated = true;

								w.Complete();
							}));
				});
		}

		public RegisterUserStateMachine()
		{
		}

		public RegisterUserStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static Event<RegisterUser> NewUserRegistration { get; set; }
		public static Event<UserValidated> EmailValidated { get; set; }

		public static State Initial { get; set; }
		public static State WaitingForEmailValidation { get; set; }
		public static State Completed { get; set; }

		public Guid CorrelationId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public bool Validated { get; set; }

		// i want to auto-wire these to a ghost consumer that just invokes the event and makes this unnecessary
		public void Consume(RegisterUser message)
		{
			RaiseEvent(NewUserRegistration, message);
		}

		// i want to auto-wire these to a ghost consumer that just invokes the event and makes this unnecessary
		public void Consume(UserValidated message)
		{
			RaiseEvent(EmailValidated, message);
		}

		public IServiceBus Bus { get; set; }
	}

	public class SagaStateMachine<T> :
		StateMachine<T>
		where T : SagaStateMachine<T>
	{
		protected SagaStateMachine()
		{
		}

		public SagaStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}