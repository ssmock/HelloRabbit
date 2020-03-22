# RabbitMQ - Concepts

## Exchanges

With Rabbit, messages must first pass through an _exchange_ to get to a queue.
An exchange is like a routing policy. Rabbit has a bunch of exchanges by default:
`direct`, `fanout`, and `topic`, among others.

You can publish a message to an exchange, but it might not be routed.

```
rabbitmqadmin publish exchange=amq.default routing_key=test payload="hello, world"
```

If `test` isn't set up, the message won't go anywhere.

## Queues

"Declaring" a queue creates it if it does not already exist. When you declare a 
queue, you can provide properties:

- name (required)
- durable - will it survive a broker restart?
- exclusive - used by a single connection, then deleted
- auto-delete - used by any number of connections, and deleted when the last one closes
- arguments - plugin-specific stuff

Note that if you try to declare an existing queue except with different attributes,
you'll get an error.

### Lazy queues

If your queue might take a while in getting to each message, it might be wise to make
it "lazy." This trades performance for reliability: messages go to disk ASAP.

## Consuming messages

When a message is delivered to a consumer, Rabbit requires confirmation. This is called
"acknowledgement," or "acking." There are a few ways to ack:

- ack, no requeue - message received, remove it (`ack_requeue_false`)
- ack, requeue - message received, but keep it in the queue in the same place it is now (`ack_requeue_true`)
- reject, requeue - message received, but rejected; keep it in place (`reject_requeue_true`)
- reject, no requeue - message received, but rejected; deadletter it (`reject_requeue_false`)

When a message is requeued, its "redeliver" flag is set to true.

