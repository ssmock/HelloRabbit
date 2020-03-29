namespace HelloRabbit.Inbound

open RabbitMQ.Client
open RabbitMQ.Client.Events

type ConnectionConfig = 
    { UserName: string
      Password: string
      VirtualHost: string
      HostName: string
      QueueName: string }

type StartReceiveResult = {
    Connection: IConnection
    Channel: IModel
    Consumer: EventingBasicConsumer
}

type ConsumerSpec = {
    Tag: string
    AckBehavior: ConsumerAckBehavior
}
and ConsumerAckBehavior =
    | Ack
    | RejectWithRequeue
    | Reject

[<RequireQualifiedAccessAttribute>]
module Receiver =

    let start (config: ConnectionConfig) consumerSpec =
        let f = ConnectionFactory()
        
        f.UserName <- config.UserName // Default: guest
        f.Password <- config.Password // Default: guest
        f.VirtualHost <- config.VirtualHost // Default: /
        f.HostName <- config.HostName // Default: localhost

        let connection = f.CreateConnection()
        let channel = connection.CreateModel()

        printfn "Connected to %s%s" config.HostName config.VirtualHost

        // Ensure that the queue exists, throwing an exception if it does not
        let declareResult = channel.QueueDeclarePassive config.QueueName

        printfn "Initialized queue %s; %d messages pending" config.QueueName declareResult.MessageCount

        let consumer = EventingBasicConsumer channel

        let handleIt (args: BasicDeliverEventArgs) =
            if args.Redelivered then
                printfn "%s received message: %A" consumerSpec.Tag args
            else
                printfn "%s received message: %A" consumerSpec.Tag args
            
            match consumerSpec.AckBehavior with
            | RejectWithRequeue ->
                printfn "......Reject, requeue"
                channel.BasicReject (args.DeliveryTag, true)
            | Reject -> 
                printfn "......Reject, no requeue"
                channel.BasicReject (args.DeliveryTag, false)
            | Ack -> 
                printfn "......Ack"
                channel.BasicAck (args.DeliveryTag, false)

        consumer.Received.Add handleIt

        // The result is just the consumer tag echoed back
        let _consumeResult = channel.BasicConsume(config.QueueName, false, consumerSpec.Tag, consumer)

        printfn "Consumer %s ready" consumerSpec.Tag

        { Connection = connection
          Channel = channel
          Consumer = consumer }
