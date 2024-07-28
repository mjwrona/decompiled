-- KEYS[i] - Redis key to be set
-- ARGV[i] - New value
-- ARGV[i+1] - Expiry (-1 makes key persistent, -2 deletes key)
-- For resiliency reasons we delete key in case of any expiry < 0 except "-1" (which makes key persistent)
-- Also since "0" is not a valid expiry we override it with minimum expiry (1 msec)

for i=1,#KEYS do
  local expiry = tonumber(ARGV[2*i])
  if expiry >= 0 then
    -- zero is not a valid expiration so bump it up to 1 msec
    redis.call('SET',KEYS[i],ARGV[2*i-1],'PX',math.max(1,expiry))
  elseif expiry == -1 then
    redis.call('SET',KEYS[i],ARGV[2*i-1])
  else
    redis.call('DEL',KEYS[i])
  end
end

return true  
