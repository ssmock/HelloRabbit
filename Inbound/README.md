## Prequisites for receipt

Here is what we need:

### 1. A connection (obvious)

### 2. A channel on which to receive messages via the connection 

For some reason this is called a "model" in the .NET SDK.

### 3. A queue

Even though we end up referring to the queue with a mere string in the actual
receipt code, we should still make sure it exists. In Rabbit-terms this is
called "declaring" a queue. The SDK gives us two options here:

A. "Actively" declaring it; this tries to create the queue if it doesn't exist
B. "Passively" declaring it; this throws an exception if it doesn't exist

## A naive "get"

The most straightforward way to get a message from a queue is like this:

```
let msg = channel.BasicGet (config.QueueName, false)
```

Where `false` indicates that the message should not be auto-acknowledged. Note 
that if there is nothing in the queue, `msg` will be null. (This surprised me;
I expected it to block until a message showed up.)

## An event-based "get"

A better way to receive messages is by wiring up a consumer. In addition to
the code being more succinct, your consumer will be registered by name ("tag"
in Rabbit-speak) with the RabbitMQ server for inspection with commands like

```
./rabbitmqadmin -c rabbitmqadmin.conf list consumers
```

Note that just creating a consumer instance and giving it an event handler is
not enough! You need to "start" it by calling:

```
channel.BasicConsume(config.QueueName, autoAck, consumerTag, consumer)
```

## Acking

After a message has been received, it should be acknowledged. If it goes without
acknowledgement, it will be returned to the queue.

This project shows three different ways to acknowledge a message:

* A basic ack; this removes it from the queue
* Basic rejection, with re-queuing; this returns it to the queue
* Basic rejection

### How to create an infinite loop

Rejecting a message and requeuing it indefinitely is allowed, but probably a bad
idea.

## Multiple receivers

Here are some personal observations.

If you define multiple consumers for a queue (at least one defined with simple
defaults), receivers seem to take turns dequeuing. Note that this happens even
with unacknowledged messages -- if a consumer gets a message, but doesn't 
acknowledge it, other consumers still can't see it. The exception here seems to
be on startup -- the first register consumer appears to get priority. I'm not
sure whether this is a rule, or contingent upon, e.g. how quickly it gets 
through its messages.

Note that my "multiple receiver" implementation uses multiple connection 
factories, etc. This probably isn't the best implementation, but it doesn't
seem to do any harm.
