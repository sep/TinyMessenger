using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyMessenger.Tests.TestData;
using TinyMessenger;
using System.Threading;

namespace TinyMessenger.Tests
{

    [TestFixtureAttribute]
    public class TinyMessengerTests
    {
        [TestAttribute]
        public void TinyMessenger_Ctor_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
        }

        [TestAttribute]
        public void Subscribe_ValidDeliverAction_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>));
        }

        [TestAttribute]
        public void SubScribe_ValidDeliveryAction_ReturnsRegistrationObject()
        {
            var messenger = UtilityMethods.GetMessenger();

            var output = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>));

            Assert.That(output, Is.InstanceOf<TinyMessageSubscriptionToken>());
        }

        [TestAttribute]
        public void Subscribe_ValidDeliverActionWIthStrongReferences_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true);
        }

        [TestAttribute]
        public void Subscribe_ValidDeliveryActionAndFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullDeliveryAction_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(null, new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullFilter_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), null, new TestProxy());
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullProxy_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), null);
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Unsubscribe_NullSubscriptionObject_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Unsubscribe<TestMessage>(null);
        }

        [TestAttribute]
        public void Unsubscribe_PreviousSubscription_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var subscription = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

            messenger.Unsubscribe<TestMessage>(subscription);
        }

        [TestAttribute]
        public void Subscribe_PreviousSubscription_ReturnsDifferentSubscriptionObject()
        {
            var messenger = UtilityMethods.GetMessenger();
            var sub1 = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));
            var sub2 = messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

            Assert.IsFalse(object.ReferenceEquals(sub1, sub2));
        }

        [TestAttribute]
        public void Subscribe_CustomProxyNoFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), proxy);
        }

        [TestAttribute]
        public void Subscribe_CustomProxyWithFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), proxy);
        }

        [TestAttribute]
        public void Subscribe_CustomProxyNoFilterStrongReference_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true, proxy);
        }

        [TestAttribute]
        public void Subscribe_CustomProxyFilterStrongReference_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), true, proxy);
        }

        [TestAttribute]
        public void Publish_CustomProxyNoFilter_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), proxy);
            var message = new TestMessage(this);

            messenger.Publish<TestMessage>(message);

            Assert.That(proxy.Message, Is.SameAs(message));
        }

        [TestAttribute]
        public void Publish_CustomProxyWithFilter_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), proxy);
            var message = new TestMessage(this);

            messenger.Publish<TestMessage>(message);

            Assert.That(proxy.Message, Is.SameAs(message));
        }

        [TestAttribute]
        public void Publish_CustomProxyNoFilterStrongReference_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), true, proxy);
            var message = new TestMessage(this);

            messenger.Publish<TestMessage>(message);

            Assert.That(proxy.Message, Is.SameAs(message));
        }

        [TestAttribute]
        public void Publish_CustomProxyFilterStrongReference_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>), true, proxy);
            var message = new TestMessage(this);

            messenger.Publish<TestMessage>(message);

            Assert.That(proxy.Message, Is.SameAs(message));
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publish_NullMessage_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Publish<TestMessage>(null);
        }

        [TestAttribute]
        public void Publish_NoSubscribers_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Publish<TestMessage>(new TestMessage(this));
        }

        [TestAttribute]
        public void Publish_Subscriber_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<TestMessage>(new Action<TestMessage>(UtilityMethods.FakeDeliveryAction<TestMessage>), new Func<TestMessage, bool>(UtilityMethods.FakeMessageFilter<TestMessage>));

            messenger.Publish<TestMessage>(new TestMessage(this));
        }

        [TestAttribute]
        public void Publish_SubscribedMessageNoFilter_GetsMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>((m) => { received = true; });

            messenger.Publish<TestMessage>(new TestMessage(this));

            Assert.IsTrue(received);
        }

        [TestAttribute]
        public void Publish_SubscribedThenUnsubscribedMessageNoFilter_DoesNotGetMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            var token = messenger.Subscribe<TestMessage>((m) => { received = true; });
            messenger.Unsubscribe<TestMessage>(token);

            messenger.Publish<TestMessage>(new TestMessage(this));

            Assert.IsFalse(received);
        }

        [TestAttribute]
        public void Publish_SubscribedMessageButFiltered_DoesNotGetMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>((m) => { received = true; }, (m) => false);

            messenger.Publish<TestMessage>(new TestMessage(this));

            Assert.IsFalse(received);
        }

        [TestAttribute]
        public void Publish_SubscribedMessageNoFilter_GetsActualMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            ITinyMessage receivedMessage = null;
            var payload = new TestMessage(this);
            messenger.Subscribe<TestMessage>((m) => { receivedMessage = m; });

            messenger.Publish<TestMessage>(payload);

            Assert.That(receivedMessage, Is.SameAs(payload));
        }

        [TestAttribute]
        public void GenericTinyMessage_String_SubscribeDoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var output = string.Empty;
            messenger.Subscribe<GenericTinyMessage<string>>((m) => { output = m.Content; });
        }

        [TestAttribute]
        public void GenericTinyMessage_String_PubishDoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));
        }

        [TestAttribute]
        public void GenericTinyMessage_String_PubishAndSubscribeDeliversContent()
        {
            var messenger = UtilityMethods.GetMessenger();
            var output = string.Empty;
            messenger.Subscribe<GenericTinyMessage<string>>((m) => { output = m.Content; });
            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));

            Assert.AreEqual("Testing", output);
        }

        [TestAttribute]
        public void Publish_SubscriptionThrowingException_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<GenericTinyMessage<string>>((m) => { throw new NotImplementedException(); });

            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));
        }

        [TestAttribute]
        public void PublishAsync_NoCallback_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.PublishAsync(new TestMessage(this));
        }

        [TestAttribute]
        public void PublishAsync_NoCallback_PublishesMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>((m) => { received = true; });

            messenger.PublishAsync(new TestMessage(this));

            // Horrible wait loop!
            int waitCount = 0;
            while (!received && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestAttribute]
        public void PublishAsync_Callback_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
#pragma warning disable 219
            messenger.PublishAsync(new TestMessage(this), (r) => {string test = "Testing";});
#pragma warning restore 219
        }

        [TestAttribute]
        public void PublishAsync_Callback_PublishesMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>((m) => { received = true; });

#pragma warning disable 219
            messenger.PublishAsync(new TestMessage(this), (r) => { string test = "Testing"; });
#pragma warning restore 219

            // Horrible wait loop!
            int waitCount = 0;
            while (!received && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestAttribute]
        public void PublishAsync_Callback_CallsCallback()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            bool callbackReceived = false;
            messenger.Subscribe<TestMessage>((m) => { received = true; });

            messenger.PublishAsync(new TestMessage(this), (r) => { callbackReceived = true; });

            // Horrible wait loop!
            int waitCount = 0;
            while (!callbackReceived && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestAttribute]
        public void CancellableGenericTinyMessage_Publish_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
#pragma warning disable 219
            messenger.Publish<CancellableGenericTinyMessage<string>>(new CancellableGenericTinyMessage<string>(this, "Testing", () => { bool test = true; }));
#pragma warning restore 219
        }

        [TestAttribute]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CancellableGenericTinyMessage_PublishWithNullAction_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Publish<CancellableGenericTinyMessage<string>>(new CancellableGenericTinyMessage<string>(this, "Testing", null));
        }

        [TestAttribute]
        public void CancellableGenericTinyMessage_SubscriberCancels_CancelActioned()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool cancelled = false;
            messenger.Subscribe<CancellableGenericTinyMessage<string>>((m) => { m.Cancel(); });

            messenger.Publish<CancellableGenericTinyMessage<string>>(new CancellableGenericTinyMessage<string>(this, "Testing", () => { cancelled = true; }));

            Assert.IsTrue(cancelled);
        }

        [TestAttribute]
        public void CancellableGenericTinyMessage_SeveralSubscribersOneCancels_CancelActioned()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool cancelled = false;
#pragma warning disable 219
            messenger.Subscribe<CancellableGenericTinyMessage<string>>((m) => { var test = 1; });
            messenger.Subscribe<CancellableGenericTinyMessage<string>>((m) => { m.Cancel(); });
            messenger.Subscribe<CancellableGenericTinyMessage<string>>((m) => { var test = 1; });
#pragma warning restore 219
            messenger.Publish<CancellableGenericTinyMessage<string>>(new CancellableGenericTinyMessage<string>(this, "Testing", () => { cancelled = true; }));

            Assert.IsTrue(cancelled);
        }

        [TestAttribute]
        public void Publish_SubscriptionOnBaseClass_HitsSubscription()
        {
            var received = false;
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<TestMessage>(tm => received = true);

            messenger.Publish(new DerivedMessage<string>(this) { Things = "Hello" });

            Assert.IsTrue(received);
        }

        [TestAttribute]
        public void Publish_SubscriptionOnImplementedInterface_HitsSubscription()
        {
            var received = false;
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<ITestMessageInterface>(tm => received = true);

            messenger.Publish(new InterfaceDerivedMessage<string>(this) { Things = "Hello" });

            Assert.IsTrue(received);
        }
    }
}