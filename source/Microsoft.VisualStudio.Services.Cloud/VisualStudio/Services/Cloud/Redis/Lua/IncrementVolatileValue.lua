-- KEYS[i] - Redis key to be incremented
-- ARGV[i] - Increment value
-- ARGV[i+1] - Expiry (negative for persistent keys)

local result = {}

for i=1,#KEYS do
  local expiry = tonumber(ARGV[2*i])
  if expiry >= 0 then
-- zero is not a valid expiration so use 1 msec
    redis.call('SET',KEYS[i],0,'PX',math.max(1,expiry),'NX')
  end

  result[i] = redis.call('INCRBY',KEYS[i],ARGV[2*i-1])
end  

return result
