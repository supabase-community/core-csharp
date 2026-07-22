using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supabase.Core.Diagnostics;

namespace CoreTests.Diagnostics
{
    [TestClass]
    public class UrlSanitizerTests
    {
        [TestMethod]
        public void Sanitize_StripsTheQueryString() =>
            UrlSanitizer.Sanitize(new Uri("https://project.supabase.co/auth/v1/token?grant_type=refresh_token&apikey=secret"))
                .Should().Be("https://project.supabase.co/auth/v1/token");

        [TestMethod]
        public void Sanitize_StripsTheFragment() =>
            UrlSanitizer.Sanitize(new Uri("https://project.supabase.co/callback#access_token=secret-jwt"))
                .Should().Be("https://project.supabase.co/callback");

        [TestMethod]
        public void Sanitize_StripsUserInfo() =>
            UrlSanitizer.Sanitize(new Uri("https://user:password@project.supabase.co/auth/v1/settings"))
                .Should().Be("https://project.supabase.co/auth/v1/settings");

        [TestMethod]
        public void Sanitize_KeepsNonDefaultPorts() =>
            UrlSanitizer.Sanitize(new Uri("http://127.0.0.1:54321/auth/v1/token?grant_type=password"))
                .Should().Be("http://127.0.0.1:54321/auth/v1/token");

        [TestMethod]
        public void Sanitize_OmitsDefaultPorts() =>
            UrlSanitizer.Sanitize(new Uri("https://project.supabase.co:443/auth/v1/settings"))
                .Should().Be("https://project.supabase.co/auth/v1/settings");

        [TestMethod]
        public void Sanitize_StringOverloadStripsQueryAndFragment() =>
            UrlSanitizer.Sanitize("https://project.supabase.co/auth/v1/verify?token=secret#fragment")
                .Should().Be("https://project.supabase.co/auth/v1/verify");
    }
}
