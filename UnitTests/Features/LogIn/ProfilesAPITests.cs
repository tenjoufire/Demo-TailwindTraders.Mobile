﻿using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TailwindTraders.Mobile.Features.LogIn;
using TailwindTraders.Mobile.Features.Settings;
using TailwindTraders.Mobile.Helpers;
using UnitTests.Framework;

namespace UnitTests.Features.LogIn
{
#if !DEBUG
    [Ignore(Constants.IgnoreReason)]
#endif
    public class ProfilesAPITests : BaseAPITest
    {
        [Test]
        public async Task GetAsync()
        {
            var profileAPI = RestService.For<IProfilesAPI>(HttpClientFactory.Create(DefaultSettings.RootWebApiUrl));
            var profiles = await PreauthenticateAsync(() => profileAPI.GetAsync(authenticationBearer));

            Assert.IsNotEmpty(profiles);
        }
    }
}
