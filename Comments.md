# Code issues in ProductApplicationService class
1. Magic numbers
Returning raw numbers instead of constant variable isn't a good option because 
we don't really know what exactly that number means. (We can also consider to switch int to enum as return type)

2. Using multiple if statements instead of if ... else if ... else or switch statement
Improving readability and quality of code

3. SubmitApplicationFor method is too big 
Better option is separate each product workflow to private helper methods or individual classes

4. Code redudancy when creating CompanyDataRequest object
It's good to have one place to instantiate class (like factors or static methods)

5. Lack of simple validations
We should check argument to avoid null exceptions (bussines validators should be on separate classes)

6. Checking Success property (additionally)
ApplicationId is nullable type so that's could means if we have null value that is no success
