# Introduction

AccountManager is a .net core 6 web application designed to run on AWS in a lambda to keep costs low by taking advantage of the AWS free tier.
It allows users to go to a web site and create accounts that can be used to log into an AnyMMO network server.

# Project Organization

The project has 3 main parts: a deploy script, a folder that contains the .net core 6 web app, and a folder that contains aws infrastructure definitions.

The deploy script will make the appropriate AWS CLI calls to install the application and its build pipeline to AWS.

# Requirements

* An AWS account and properly configured AWS CLI : https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html
* A MySQL database.  Currently, this project does not setup any database on AWS.

# Application Setup

To install or run the application, the database must be configured.  Running the entity framework migration tool as described below will create and configure the database tables.

## Add database connection string to appsettings

Copy appsettings.json to appsettings.Development.json and add a connection string, using the proper values for your database.

```
"ConnectionStrings": {
    "Db": "server=mysql.yourdomain.com;database=your_database_name;user=your_database_user_name;password=your_database_password"
  },
```

## Update database tables

Open Powershell

`dotnet tool install --global dotnet-ef --version 6.*`

Replace the word Initial with a database commit message

`dotnet ef migrations add Initial`

Look at the new file in the Migrations folder in visual studio and check the code that will be run.  If it looks good, then run the update.

`dotnet ef database update`

# AWS Setup

Create a codestar connection to monitor the github repository (if you have never connected a github repo to your AWS account before) : https://docs.aws.amazon.com/dtconsole/latest/userguide/connections-create-github.html

Create an SSM parameter with the name `/app/AccountManager/CodestarConnectionArn` and put the arn of the codestar connection in it

Create an SSM parameter with the name `/app/AccountManager/DatabaseConnectionString` and put the database connection string in it

Create an SSM parameter with the name `/app/AccountManager/BearerKey` and put any secret string in it

Run `./deploy.sh`

The first time the cloudformation runs, an SSL certificate will be created by AWS.  In order for that certificate to be verified, you will need to look at Route53 and look at the newly created domain, account.yourdomain.com.  You will need to add the NS servers from that domain to the DNS records in the top level domain, yourdomain.com.  Until that is done, aws will not be able to find the custom CNAME record it creates in account.yourdomain.com, and the certificate will not finish creating.