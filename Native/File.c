#include <emscripten.h>

EM_ASYNC_JS(void, syncDatabase, (int populate), {
	console.log('syncDatabase Promise');
	await new Promise(
		function(resolve, reject)
	{
		console.log('syncDatabase Promise ok');
		FS.syncfs(populate != 0, function(err){
			console.log('syncDatabase FS.syncfs');
			if (err) {
				console.log('syncfs failed. Error:', err);
				reject(err);
			}
			else {
				console.log('synced successfull.');
				resolve();
			}
		});
	});
	});

void mountAndInitializeDb()
{
	EM_ASM(
		{
			console.log('mountAndInitializeDb');
			FS.mkdir('/database');
			FS.mount(IDBFS, {}, '/database');
		});

	return syncDatabase(1);
}
