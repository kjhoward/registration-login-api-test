using NUnit.Framework;
using AutoMapper;
using System.Threading.Tasks;
using System;
using RegistrationLoginApi.Data;
using RegistrationLoginApi.Test.Mapping;
using System.Linq;
using BCryptNet = BCrypt.Net.BCrypt;
using DevConsulting.RegistrationLoginApi.Models;
using RegistrationLoginApi.Data.DataModels;
namespace RegistrationLoginApi.Test;

public class RegistrationLoginRepositoryTest
{
    private IRepository _repo;
    private IAppDbContext _context;
    private IMapper _mapper;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _context = await SetupHelper.SetupDbContext("RepoTests", true);
        var config = MappingProfile.GetMapperConfiguration();
        _mapper = config.CreateMapper();
        _repo = new Repository(_context,_mapper);
    }

    [Test]
    public void TestGetAllUsers_ExpectPass()
    {
        var queryResult = _repo.GetAllUsers();
        Assert.AreEqual(1,queryResult.TotalItems);
    }

    [Test]
    public void TestGetUser_By_Id_ExpectPass()
    {
        var queryResult = _repo.GetUser(1);
        Assert.AreEqual(1,queryResult.TotalItems);

        var user = queryResult.Items.FirstOrDefault();
        Assert.AreEqual("tester", user.Username);
        Assert.AreEqual("bob", user.FirstName);
        Assert.AreEqual("builder", user.LastName);
        Assert.IsTrue(BCryptNet.Verify("test", user.PasswordHash));
    }

    [Test]
    public void TestGetUser_By_Username_ExpectPass()
    {
        var queryResult = _repo.GetUser("tester");
        Assert.AreEqual(1,queryResult.TotalItems);

        var user = queryResult.Items.FirstOrDefault();
        Assert.AreEqual("tester", user.Username);
        Assert.AreEqual("bob", user.FirstName);
        Assert.AreEqual("builder", user.LastName);
        Assert.IsTrue(BCryptNet.Verify("test", user.PasswordHash));
    }

    [Test]
    public async Task TestAddUserAsync_ExpectPass(){
        //Create new context, otherwise our tests using our current ones may fail if this is ran first.
        var localContext = await SetupHelper.SetupDbContext("RepoTests2", true);
        var tempRepo = new Repository(localContext,_mapper);
        var user = new UserResource{
            Id = 2,
            Username = "tester2",
            FirstName = "billy",
            LastName = "builder",
            PasswordHash = BCryptNet.HashPassword("tester")
        };
        await tempRepo.AddUserAsync(user);

        var queryResult = tempRepo.GetAllUsers();
        Assert.AreEqual(2,queryResult.TotalItems);
    }
    [Test]
    public async Task TestRemoveUserAsync_ExpectPass(){
        //Create new context, otherwise our tests using our current ones may fail if this is ran first.
        var localContext = await SetupHelper.SetupDbContext("RepoTests3", true);
        var tempRepo = new Repository(localContext,_mapper);
        var user = tempRepo.GetUser(1).Items.FirstOrDefault();
        await tempRepo.RemoveUserAsync(user);

        var queryResult = tempRepo.GetAllUsers();
        Assert.AreEqual(0,queryResult.TotalItems);
    }

    [Test]
    public async Task TestUpdateUserAsync_ExpectPass(){
        var user = _repo.GetUser(1).Items.FirstOrDefault();
        user.FirstName = "Updated";
        await _repo.UpdateUserAsync(user);

        user = _repo.GetUser(1).Items.FirstOrDefault();
        Assert.AreEqual("Updated", user.FirstName);
    }
}