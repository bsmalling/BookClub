using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Server.Areas.Identity;
using Server.Data;
using System.Text;

using BookClub.Common;
using BookClub.Services;
using BookClub.Services.Interfaces;
using BookClub.Contexts;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
{ // Begin local scope to prevent user secrets in memory.
#if DEBUG
    var admin_cs = builder.Configuration.GetConnectionString("AdminBCDBCS") +
        "Password=" + builder.Configuration["BOOKCLUB_ADMIN_PWD"] + ";";
    builder.Services.AddDbContextFactory<AdminBookClubContext>(
        options => options.UseSqlServer(admin_cs));

    var user_cs = builder.Configuration.GetConnectionString("UserBCDBCS") +
        "Password=" + builder.Configuration["BOOKCLUB_USER_PWD"] + ";";
    builder.Services.AddDbContextFactory<UserBookClubContext>(
        options => options.UseSqlServer(user_cs));
#else
    var admin_cs = builder.Configuration.GetConnectionString("AZAdminBCDBCS") +
        "Password=" + builder.Configuration["AZBOOKCLUB_ADMIN_PWD"] + ";";
    builder.Services.AddDbContextFactory<AdminBookClubContext>(
        options => options.UseSqlServer(admin_cs));

    var user_cs = builder.Configuration.GetConnectionString("AZUserBCDBCS") +
        "Password=" + builder.Configuration["AZBOOKCLUB_USER_PWD"] + ";";
    builder.Services.AddDbContextFactory<UserBookClubContext>(
        options => options.UseSqlServer(user_cs));
#endif
} // End local scope to prevent user secrets in memory.

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<UserBookClubContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(); // .AddCircuitOptions(option => { option.DetailedErrors = true; });
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
builder.Services.AddScoped<IBookService, BookService>(ServiceFactories.CreateBookService);
builder.Services.AddScoped<ICommentService, CommentService>(ServiceFactories.CreateCommentService);
builder.Services.AddScoped<ILocationService, LocationService>(ServiceFactories.CreateLocationService);
builder.Services.AddScoped<IMeetingService, MeetingService>(ServiceFactories.CreateMeetingService);
builder.Services.AddScoped<IInvitationService, InvitationService>(ServiceFactories.CreateInvitationService);
builder.Services.AddScoped<IRecommendationService, RecommendationService>(ServiceFactories.CreateRecommendationService);
builder.Services.AddScoped<IThumbnailService, ThumbnailService>(ServiceFactories.CreateThumbnailService);
builder.Services.AddScoped<IUserService, UserService>(ServiceFactories.CreateUserService);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

internal static class ServiceFactories
{

    public static BookService CreateBookService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new BookService(context);
    }

    public static CommentService CreateCommentService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new CommentService(context);
    }

    public static LocationService CreateLocationService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new LocationService(context);
    }

    public static MeetingService CreateMeetingService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new MeetingService(context);
    }

    public static InvitationService CreateInvitationService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new InvitationService(context);
    }

    public static RecommendationService CreateRecommendationService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new RecommendationService(context);
    }

    public static ThumbnailService CreateThumbnailService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new ThumbnailService(context);
    }

    public static UserService CreateUserService(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<UserBookClubContext>();
        return new UserService(context);
    }

}
