#!/bin/sh

# Constants
PASSES=20000
PROJECT=..
HOST=localhost:5000
CAL=calibrate
PRO=process
PERF=./ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.sh

$PERF -n $PASSES -s "dotnet run --project $PROJECT" -c $CAL -p $PRO -h $HOST
