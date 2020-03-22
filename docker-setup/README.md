# run-one-off.sh

From https://registry.hub.docker.com/_/rabbitmq/

Runs a one-off rabbitmq container, listening on port 5672, with the management plugin attached.

## The management page

First, get the IP: 

```
docker inspect --format='{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' some-rabbit
```

Then open the page at http://<the-ip>:15672. The default credentials are guest/guest.

### Things you can do from the management page

Among other things:

- Configure exchanges
- Configure queues
- Do stuff with the queues: inspect, publish, get, purge

## CLI

`rabbitmqadmin` is a CLI for managing the instance; it's an alternative to the management web page.
The easiest way to get it is to actually download it form the management page, at 
http://<the-ip>:15672/cli/rabbitmqadmin.

After downloading it, you will probably need to `chmod` it before running it.

### Primary options

You use the CLI by sending it _subcommands_, but you also need to specify some options for connecting.

```
-H <ip> - the host IP (default "15672")
-P <port> - the host port
-u <user> - the user (default "guest")
-p <pass> - the password (default "guest")
```

There are also options for connecting with SSL.

Alternatively, you can use a `rabbitmqadmin.conf` file that contains options. This directory contains one
to make it easier to connect to `some-rabbit`. Note that the IP address might need to be changed.

```
./rabbitmqadmin -H 172.17.0.3 list queues
```

### Doing things with it

```
./rabbitmqadmin -c rabbitmqadmin.conf list queues
```

```
./rabbitmqadmin -c rabbitmqadmin.conf declare queue name=test durable=true
```

```
/rabbitmqadmin -c rabbitmqadmin.conf publish exchange=amq.default routing_key=test payload="bad news"
```

```
./rabbitmqadmin -c rabbitmqadmin.conf get queue=test ackmode=ack_requeue_true
```

`ack_requeue_true` is the default. Other `ackmode`s: `ack_requeue_false`, `reject_requeue_true`, `reject_requeue_false`.