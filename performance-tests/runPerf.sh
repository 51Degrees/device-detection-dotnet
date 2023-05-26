#!/bin/sh

# Constants
PASSES=20000
PROJECT=..
HOST=localhost:5000
CAL=calibrate
PRO=process
PERF=./ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.sh

pwd

ls ..

$PERF -n $PASSES -s "dotnet run --project $PROJECT" -c $CAL -p $PRO -h $HOST
