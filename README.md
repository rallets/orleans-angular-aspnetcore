# orleans-angular-aspnetcore

## **_This is a draft_**

---

This is test-demo project on [Microsoft Orleans](https://github.com/dotnet/orleans) and its capabilities beeing used in a SPA application and a classical N-tier-approach like.
I have only found example projects using basic IoT or games application, that fit (too) perfectly for Orleans.
My goal is to try to convert the multi-tier approach (SPA, WebApi, Sql Server) to an Actor-oriented one, and be able to understand its limits, where it fits very well, and where is not. For doing this I'm trying to implement patterns and best-practice in a simple way, to make easy to understand for everyone.
I've on purpose keep the Angular/SPA/html part simple (no Redux, no fancy html/scss, etc), because that's just needed to interact with the backend.
The [Orleanse documentation](https://dotnet.github.io/orleans/Documentation/) is a good place to start, but as today it lacks a lot of knowledge that instead I have found arount in the web, or asking in the [gitter channel](https://gitter.im/dotnet/orleans).

To run the project you can take a look to the file [setup.md](.\src\00-Documentation\setup.md)

I welcome feedback!

## Final considerations

I think that Orleans is a beautiful framework and it was really fun to work with, but unfortunatly **it lacks documentation** and **real simple examples**: this make difficult to start with it. It's on heavy development, and it's production-ready for projects that fit in the Actor pattern like "micro/nano-services" or "DDD aggregates". The design should be done [Avoiding Microservice Megadisasters (Jimmy Boggart)](https://www.youtube.com/watch?v=gfh-VCTwMw8).
As today it's not ready for a more DBMS/multi-tier replacement, but they are working on a AODB-Indexing implementation, hopefully coming soon. More info are available on the [Orleans Indexing](https://github.com/OrleansContrib/OrleansV2.Fork.Indexing) fork.
A research paper describing how it will work can be found [here](http://cidrdb.org/cidr2017/papers/p29-bernstein-cidr17.pdf). I think it's a must to read to understand how Indexing in Orleans works.

My idea is that Orleans **can work** very well if you have:

1. a lot of actors with short lifetime, and you don't need to query agains them often. An example could be: payment transaction, orders, etc.
2. limited amount of actors with long lifetime, and you need to query agains them often. An example: products, inventory, warehouses, etc.

I think it will **not work** very well if you have:

1. A classical CRM/ERP application, with big numbers of actors, and you need to query against them normally. (*1)
2. the need of changing the models/storage/domain or if it's unclear how the data rapresentation should be (*2)

(*1) The main limitation is the Indexing part, where the in-memory indexes could be bigger than the available RAM. Imagine that you need to index 20 differents actor-types, and you have 10 properties each, and you could have thausand or millions of actors: this means at you will need to keep huge in-memory dictionary to be able to query them. If you need to query through the db to get a search-result, then Orleans will not be leveraged.

(*2) **It must be undertood very well the serialization strategy**: it's pretty costly to extend or change a model to persist the actor-state, because **the default serializer is not version tolerant** yet. Then you need to use Google Protobuf, ProtobufNet, Microsoft Bold, or your own implementation. There is a work-in-progress for a default-serializer more version tolerant.

## Details

### Patterns and best practices

[Orleans Best practice (pdf)](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/Orleans20Best20Practices.pdf)
[Orleans Design patterns](https://github.com/OrleansContrib/DesignPatterns)
[Orleans Architecture Patterns - NDC Sydney](https://vimeo.com/187705479)
[Orleans: Distributed Virtual Actors for Programmability and Scalability](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/Orleans-MSR-TR-2014-41.pdf)
[Distributed Transactions are dead, long live distributed transaction! by Sergey Bykov](https://www.youtube.com/watch?v=8A5bRdyZXJw)

This project is trying to implement those patterns (from the youtube video):

* Registry pattern: keep a list of items (for search)
* Smart cache: optimize read and writes (for single grain)
* Event sourcing
* Aggregate pattern (he said it's very complex, hundred of lines of code) - to have a projection (registry + ES pattern)
* Stream: listen to message in a bus (can be a "new order" command?)

## Orleans concepts

### Grain persistence

You can store your grain-state basically wher you want. In my example I'm using:

* Azure Table Storage (*1)(*2): cheap but 64kb limit. Good fit for normal grains, not for indexing-grains (if a index-dictionary can grow more than 64kb)
* Azure Blob Storage (*2): more expansive but no storage limits. Good fit to store indexing grains.
* (TBD) Microsoft Sql Server RDBM: [relational storage](https://dotnet.github.io/orleans/Documentation/grains/grain_persistence/relational_storage.html) is possible. This is perfect if you need to make your data available to other systems, or you need to re-use existing data.

You can also use in-memory storage, very fast, but real-world applications using this approach are very uncommon.

(*1) In the orleans cluster configuration you need to specify a ServiceId (ex: "OrleansTest"). If you change it **you will lose all the states stored** using "Azure Table Storage" (you need to fix the string-Ids manually).
(*2) if you change a namespace or a class name of a model you are persisting, **you will lose all states stored** using "Azure Table Storage" and "Azure Blob Storage"

### Serializers

A serializer is needed when you need to comunicate between the Actor and external components. The state is then serialized and the unserialized. Serialization is needed to save the state on disk to be able to recover it in future.
That means, if you are using the default serializer, that if you change a model state, that will raise errors: the default serializer is not version tolerant yet.
It's great for prototyping at the speed of light, however, once you commit to a model, it's best to switch to a version tolerant serializer like Protobuf or Bond: [those are supported out of the box](https://dotnet.github.io/orleans/Documentation/core_concepts/what_are_orleans_packages.html#serializers).
There's the [serialization page](https://dotnet.github.io/orleans/Documentation/clusters_and_clients/configuration_guide/serialization.html) but it's outdated. A github [issue](https://github.com/dotnet/orleans/issues/5492) was opened about this topic.

I have struggled a lot to have something working. Basically for each model that you want to serialize, you need to add some decorators to tell the serializator the serialization strategy.
As example:

* how should I serialize a DateTime? there are plenty of different implementations... here you shouls decide what you want. Keep in mind that *serialization* and *speed* are not friends!
* which position has this property in the serialized object?

1. **[Bond](https://github.com/Microsoft/bond)** out-of-the-box implementation has only some base types available (no decimal, no Guid, no DateTimeOffset support out-of-the-box) and it's really cryptic to use... I was able to extend a bit but after some hours I was very far from see the light...
2. I was not able to use **[Google protobuf](https://github.com/protocolbuffers/protobuf)**, no idea on how to set the right attributes...
3. **[ProtobufNet](https://github.com/mgravell/protobuf-net)** is better, but missing some types like DateTimeOffset too... then it requires some custom code around (*DateTimeOffsetSurrogate* and *RuntimeTypeModel.Default.Add(...)*), but it was the faster and easier to setup correcty, supporting new properties on already-serialized object.

So I think that newcomers should understand very well the risk in the long-term of using the default serializer.

### Messaging and at-least-once garantee

The messaging system of Orleans doesn't garantee that you will receive at-least-one message, because that will cost on terms of speed. If you really need that, you can use a Persistent Queue (like Azure Service Bus or RabbitMQ) and keep listening to the messages using a Observer pattern.

My example use a [Azure Service Bus Queue](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-portal) because the *Basic pricing tier* is close to free and you will pay for message.
You could find useful [ServiceBusExplorer](https://github.com/paolosalvatori/ServiceBusExplorer).

You will need to configure your settings using:

* primary key: [...44 chars...]
* primary connection string: Endpoint=sb://xxx.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;* SharedAccessKey=[...44 chars...]
* queue name: orleans-order-create-queue

### Distributed transactions

[TBD]

Orleans supports distributed transactions, that means that you can manage multiple actors in a transaction. Take a look to this youtube video for more details.

### Co-hosting cluster and http client

This can be achieved using the Hosted client, enabled by default from Orleans 2.3. The documentation is coming, take a look at the [github issue here](https://github.com/dotnet/orleans/issues/5144). In the meantime take a look to this [stackoverflow thread](https://stackoverflow.com/questions/54841844/orleans-direct-client-in-asp-net-core-project/54842916#54842916).
That means a silo and a http api may be the same process, so no serialization would be done in that case.

### Other links

[Aggregating results in Orleans](https://coderead.wordpress.com/2014/06/10/aggregating-results-in-orleans/)
[Creating RESTful Services using Orleans](https://caitiem.com/2014/04/04/creating-restful-services-using-orleans/)
[Building a realtime server backend using the Orleans Actor system, Dotnet Core and Server-side Redux](https://medium.com/@MaartenSikkema/using-dotnet-core-orleans-redux-and-websockets-to-build-a-scalable-realtime-back-end-cd0b65ec6b4d)
[Developing APIs using Actor model in ASP.NET Core](https://samueleresca.net/2018/07/developing-apis-using-actor-model-in-asp-net/)
[Building IoT Solutions with Microsoft Orleans and Microsoft Azure - Part 2](https://rahulrai.in/post/building-iot-solutions-with-microsoft-orleans-and-microsoft-azure---part-2/)
