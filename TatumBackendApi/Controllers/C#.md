1. Draft Entities First
2. Create the services or repository, but repository is advisable
3. Abstraction. All the services and repository 


Most important
    - Controllers, Entities and DTOs

Services handles the business rules and logics
Repositories helps you interact with your database, it is a contract.
Interfaces defines the methods that will be used by the repositories to interact with the database 
IEnumerable is the base class for all attributes
IEnumerable is also an iterable
List extends IEnumerable

Read on Solid principles
S - Single Responsibility Principle (SRP)
O - Open or Closed Principle (OCP)
L - Liskov Substitution Principle (LSP)
I - Interface Segregation Principle (ISP)
D - Dependency Inversion principle (DIP)