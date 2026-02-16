namespace Tests;

public class UnitTest1
{

    // what do I want to test?
    // mock TaskService with an in-memory db inserted into the context

    // to do this we need to accept an interface for the TaskContext so that we can use a mock with inmemory db
    
    [Fact]
    public void Test1()
    {

        // create a test TaskContext with in memory context
        // options.UseInMemoryDatabase("TestDb")

        // create a TaskService with the TaskContext
        // what do we even want to test?
        // the list, add and remove methods

        // what cases are we even testing?
        // there is so little logic to test here
        // so much of the fuctionality is in the db


        Assert.True(true);
    }
}

