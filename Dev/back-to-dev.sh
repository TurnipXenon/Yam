# refresh main and go back to the current branch with the same HEAD as main
# reference: https://stackoverflow.com/q/1417957/17836168
turnip_last_branch=$(git branch | awk '/\*/ { print $2; }')
git checkout main && git pull origin main && git branch -d "$turnip_last_branch" && git checkout -b "$turnip_last_branch"
