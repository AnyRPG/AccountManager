#! /bin/bash

PRODUCT_NAME="AccountManager"
GIT_BRANCH=`git branch --show-current`

# GET PARAMETERS FROM THE USER
read -p "Git Owner [AnyRPG] : " GIT_OWNER
GIT_OWNER=${GIT_OWNER:-AnyRPG}
echo "Using Git Owner ${GIT_OWNER}"
read -p "Git Repo [AccountManager]: " GIT_REPO
GIT_REPO=${GIT_REPO:-AccountManager}
echo "Using Git Repo ${GIT_REPO}"
read -p "Git Branch [${GIT_BRANCH}] : " GIT_SELECTED_BRANCH
GIT_BRANCH=${GIT_SELECTED_BRANCH:-${GIT_BRANCH}}
echo "Using Git Branch ${GIT_BRANCH}"

# DEPLOY IAM PERMISSIONS
echo "Deploying Pipeline IAM permissions..."
aws cloudformation deploy --template-file cfn/iam-permissions.yaml --stack-name ${PRODUCT_NAME}-${GIT_BRANCH}-iam-permissions --parameter-overrides GitBranch=${GIT_BRANCH} --capabilities CAPABILITY_IAM --capabilities CAPABILITY_NAMED_IAM

# DEPLOY S3 BUCKET WITH LAMBDA CODE
echo "Deploying artifacts bucket..."
aws cloudformation deploy --template-file cfn/artifacts-bucket.yaml --stack-name ${PRODUCT_NAME}-${GIT_BRANCH}-artifacts-bucket --parameter-overrides GitBranch=${GIT_BRANCH}

echo "Uploading default lambda zip to bucket if missing..."
ARTIFACT_EXISTS=$(aws s3 ls accountmanager-${GIT_BRANCH}-pipeline-artifacts-bucket)
if [ -z "$ARTIFACT_EXISTS" ]; then
  echo "Zip does not exist, building and uploading"
  dotnet build app/AccountManager/AccountManager/AccountManager.csproj
  (cd app/AccountManager/AccountManager/bin/Debug/net6.0/ && zip -r AccountManager.zip .)
  aws s3 cp app/AccountManager/AccountManager/bin/Debug/net6.0/AccountManager.zip s3://accountmanager-master-pipeline-artifacts-bucket/AccountManager.zip
else
  echo "Zip already exists, continuing deployment"
fi

# DEPLOY PIPELINE
echo "Deploying pipeline..."
aws cloudformation deploy --template-file cfn/pipeline.yaml --stack-name ${PRODUCT_NAME}-${GIT_BRANCH} --parameter-overrides GitBranch=${GIT_BRANCH} GitOwner=${GIT_OWNER} GitRepo=${GIT_REPO} --capabilities CAPABILITY_IAM --capabilities CAPABILITY_NAMED_IAM