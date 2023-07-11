#! /bin/bash

PRODUCT_NAME="AccountManager"
GIT_BRANCH=`git branch --show-current`

# GET PARAMETERS FROM THE USER
read -p "Git Owner : " GIT_OWNER
read -p "Git Repo : " GIT_REPO
read -p "Git Branch [${`git branch --show-current`}] : " GIT_BRANCH
GIT_BRANCH=${GIT_BRANCH:-`git branch --show-current`}

# DEPLOY PIPELINE
aws cloudformation deploy --template-file cfn/pipeline.yaml --stack-name ${PRODUCT_NAME}-${GIT_BRANCH} --paramter-overrides GitOwner=${GIT_OWNER} GitRepo=${GIT_REPO}